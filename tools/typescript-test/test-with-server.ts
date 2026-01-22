/**
 * Test to verify if Firefox BiDi sends duplicate BeforeRequestSent events
 * This version uses an actual HTTP server like the PuppeteerSharp test.
 *
 * Run with: npm install && npm test
 */

import puppeteer from 'puppeteer';
import http from 'http';
import { execSync } from 'child_process';

interface RequestInfo {
  requestId: string;
  url: string;
  method: string;
  timestamp: number;
}

async function main() {
  console.log('=== Firefox BiDi Duplicate Request Test (with Server) ===\n');

  // Ensure Firefox is installed
  console.log('Ensuring Firefox is installed...');
  try {
    execSync('npx puppeteer browsers install firefox', { stdio: 'inherit' });
  } catch (e) {
    console.log('Firefox installation may have already completed or failed, continuing...');
  }
  console.log('');

  // Create a simple HTTP server
  const server = http.createServer((req, res) => {
    console.log(`[SERVER] ${req.method} ${req.url}`);

    if (req.url === '/one-style.html') {
      res.writeHead(200, { 'Content-Type': 'text/html' });
      res.end(`<link rel='stylesheet' href='./one-style.css'>\n<div>hello, world!</div>`);
    } else if (req.url === '/one-style.css') {
      res.writeHead(200, { 'Content-Type': 'text/css' });
      res.end('body { background: red; }');
    } else {
      res.writeHead(404);
      res.end('Not found');
    }
  });

  await new Promise<void>((resolve) => server.listen(8765, resolve));
  console.log('Test server running on http://localhost:8765\n');

  // Launch browser with BiDi protocol
  // Change to 'firefox' to test Firefox, 'chrome' to test Chrome
  const browserType = (process.env.BROWSER || 'firefox') as 'firefox' | 'chrome';
  console.log(`Using browser: ${browserType}\n`);

  const browser = await puppeteer.launch({
    browser: browserType,
    protocol: 'webDriverBiDi',
    headless: true,
  });

  const page = await browser.newPage();

  // Track all requests
  const allRequests: RequestInfo[] = [];
  const requestsByUrl = new Map<string, RequestInfo[]>();

  // Enable request interception
  await page.setRequestInterception(true);

  // Listen to all requests
  page.on('request', (request) => {
    const info: RequestInfo = {
      requestId: request.id,
      url: request.url(),
      method: request.method(),
      timestamp: Date.now(),
    };

    allRequests.push(info);

    // Group by URL
    const urlRequests = requestsByUrl.get(info.url) || [];
    urlRequests.push(info);
    requestsByUrl.set(info.url, urlRequests);

    console.log(`[REQUEST] ID: ${info.requestId} URL: ${info.url}`);

    // Abort CSS requests, continue others (same as the failing test)
    if (request.url().endsWith('.css')) {
      console.log(`  -> ABORTING CSS request`);
      request.abort('failed', 0);
    } else {
      request.continue({}, 0);
    }
  });

  // Track failed requests
  let failedCount = 0;
  page.on('requestfailed', (request) => {
    failedCount++;
    console.log(`[FAILED #${failedCount}] ID: ${request.id.substring(0, 20)}... URL: ${request.url()}`);
  });

  // Navigate to the test page
  console.log('\n--- Navigating to http://localhost:8765/one-style.html ---\n');

  const response = await page.goto('http://localhost:8765/one-style.html');

  console.log(`\n--- Navigation completed ---`);
  console.log(`Response OK: ${response?.ok()}`);
  console.log(`Response URL: ${response?.url()}\n`);

  // Analysis
  console.log('\n=== ANALYSIS ===\n');
  console.log(`Total requests captured: ${allRequests.length}`);
  console.log(`Total failed requests: ${failedCount}`);

  // Check for duplicates
  console.log('\n--- Requests grouped by URL ---\n');
  let hasDuplicates = false;
  for (const [url, requests] of requestsByUrl) {
    const shortUrl = url.length > 60 ? url.substring(0, 60) + '...' : url;
    if (requests.length > 1) {
      hasDuplicates = true;
      console.log(`DUPLICATE FOUND for URL: ${shortUrl}`);
      console.log(`  Count: ${requests.length}`);
      for (const req of requests) {
        console.log(`    - ID: ${req.requestId}`);
      }
    } else {
      console.log(`Single request for: ${shortUrl}`);
      console.log(`    - ID: ${requests[0].requestId}`);
    }
  }

  console.log('\n=== CONCLUSION ===\n');
  if (hasDuplicates) {
    console.log('CONFIRMED: Firefox BiDi sends duplicate BeforeRequestSent events with different request IDs!');
    console.log('This explains why the PuppeteerSharp test fails with 2 failed requests instead of 1.');
  } else {
    console.log('No duplicates detected in this test run.');
  }

  if (failedCount !== 1) {
    console.log(`\nTEST RESULT: Expected 1 failed request, but got ${failedCount}`);
    if (failedCount > 1) {
      console.log('This confirms the duplicate request issue!');
    }
  } else {
    console.log(`\nTEST RESULT: Got exactly 1 failed request as expected.`);
  }

  await browser.close();
  server.close();
}

main().catch(console.error);
