---
name: Bidi migration
description: Implement bidi code
---

# Context
You are a software developer integrating changes from a project written in TypeScript into a .NET project.
We call upstream to the original typescript project.

# Task

You are going to implement the changes to make $ARGUMENTS pass.

In order to do that you will need to:
 * Find the entry with the `testIdPattern` $ARGUMENTS in TestExpectations.local.json and remove that entry.
 * You are going to look for tests that might match that `testIdPattern` but looking at the test attribute.
    - For instance if the `testIdPattern` is `Chromium-Specific *`, `[Test, PuppeteerTest("chromiumonly.spec", "Chromium-Specific Page Tests", "Page.setRequestInterception should work with intervention headers")]` will be a valid match.
 * Once you see what fail in those tests you will need to modify the code to make the test pass. You should not edit the tests themselves.
 * You will find the code in typescript implementing the feature at ../../../puppeteer/puppeteer and you will need to port that code to .NET.
 * You will also find BidiDriver code at ../../../webdriverbidi-net/webdriverbidi-net you need to check what you need to use from there to make the tests pass.
 * You will have to implement the code as close as possible to the original code, but adapted to .NET idioms and practices.
 * As part of the task you will need to generate a document explaining the changes you made, and how they relate to the original PR.
 * You need to run related tests to ensure everything is working as expected.
 * You will need to run the tests using the ENV variables BROWSER=FIREFOX and PROTOCOL=bidi
 * This task should use the v21 branch as base branch and to create pull requests.
