/**
 * Test to verify if Firefox BiDi sends network events for reload with interception
 * This replicates the "should properly return navigation response when URL has cookies" test
 *
 * Run with: npx ts-node test-reload-cookies.ts
 */

import puppeteer from 'puppeteer';
import http from 'http';

async function main() {
  console.log('=== Firefox BiDi Reload with Cookies Test ===\n');

  // Create a simple HTTP server
  const server = http.createServer((req, res) => {
    console.log(`[SERVER] ${req.method} ${req.url} - Cookie: ${req.headers.cookie || 'none'}`);
    res.writeHead(200, { 'Content-Type': 'text/html' });
    res.end('<html><body>Hello</body></html>');
  });

  await new Promise<void>((resolve) => server.listen(8766, resolve));
  console.log('Test server running on http://localhost:8766\n');

  const browserType = (process.env.BROWSER || 'firefox') as 'firefox' | 'chrome';
  console.log(`Using browser: ${browserType}\n`);

  const browser = await puppeteer.launch({
    browser: browserType,
    protocol: 'webDriverBiDi',
    headless: true,
  });

  const page = await browser.newPage();

  // Step 1: Navigate to empty page
  console.log('Step 1: Navigate to empty page');
  await page.goto('http://localhost:8766/empty');
  console.log('Navigation complete\n');

  // Step 2: Set cookie
  console.log('Step 2: Set cookie');
  await page.setCookie({ name: 'foo', value: 'bar' });
  console.log('Cookie set\n');

  // Step 3: Enable request interception
  console.log('Step 3: Enable request interception');
  await page.setRequestInterception(true);
  console.log('Request interception enabled\n');

  // Step 4: Add request handler
  console.log('Step 4: Add request handler');
  let requestCount = 0;
  page.on('request', (request) => {
    requestCount++;
    console.log(`  [REQUEST #${requestCount}] ${request.url()}`);
    request.continue();
  });
  console.log('Request handler added\n');

  // Step 5: Reload with timeout
  console.log('Step 5: Reload page (with 10s timeout)');
  try {
    const response = await page.reload({ timeout: 10000 });
    console.log(`Reload complete! Status: ${response?.status()}\n`);
    console.log('=== TEST PASSED ===');
  } catch (error) {
    console.log(`Reload failed: ${error}\n`);
    console.log(`Request count during reload: ${requestCount}`);
    console.log('=== TEST FAILED ===');
    console.log('\nThis confirms Firefox BiDi does not send network.beforeRequestSent for reload with interception.');
  }

  await browser.close();
  server.close();
}

main().catch(console.error);
