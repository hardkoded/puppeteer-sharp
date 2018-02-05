# Introduction

This project is a .NET port of the official [Node.JS Puppeteer API](https://github.com/GoogleChrome/puppeteer). 
The first goal is to get a functional library with an API as close as the official one.

# Monthly reports

 * [February 2018](http://www.hardkoded.com/blogs/puppeteer-sharp-monthly-february-2018)

# Roadmap
Getting to all the 523 tests Puppeteer has, will be a long and fun journey. So, this will be the roadmap for Puppeteer Sharp 1.0:

## 0.1 First Minimum Viable Product
The first 0.1 will include:
* Browser download
* Basic browser operations: create a browser, a page and navigate a page.
* Take screenshots.
* Print to PDF.

## 0.2 Repository cleanup
This version won't include a new version. It will be about improving the repository:

* Setup CI.
* Create basic documentation (Readme, contributing, code of conduct).

## 0.3 Puppeteer
It will implement all [Puppeteer related tests](https://github.com/GoogleChrome/puppeteer/blob/master/test/test.js#L108)

## 0.4 Page
It will implement all Page tests except the ones testing the evaluate method.
As this will be quite a big version, I think we will publish many 0.3.X versions before 0.4.

## 0.5 Frames
It will implement all Frame tests.

## 0.6 Simple interactions
It will implement all the test related to setting values to inputs and clicking on elements.

## 0.X Intermediate versions
At this point, We will have implemented most features, except the ones which are javascript related.
I believe there will be many versions between 0.6 and 1.0.

## 1.0 Puppeteer the world!
The 1.0 version will have all (or most) Puppeteer features implemented. I don't know if we'll be able to cover 100% of Puppeteer features, due to differences between both technologies, but we'll do our best.

# Progress

* Tests on Google's Puppeteer: 523
* Tests on Puppeteer Sharp: 1
* Passing tests: 1

I know, this sounds pretty lame, but we will get there.
