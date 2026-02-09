# Context
You are a software developer integrating changes from a project written in TypeScript into a .NET project.
We call upstream to the original typescript project.

# Task

You are going to check the changes on the PR $ARGUMENTS
You will find the upstream repository at ../../puppeteer/puppeteer. Feel free to read the code there and even make git action to bring the changes you need to evaluate.

Once you have all the context, you will implement the same changes in this .NET project.
You will create a new branch named `implement-upstream-change-<PR_NUMBER>` where `<PR_NUMBER>` is the number of the PR you are implementing.
You will have to implement the code as close as possible to the original code, but adapted to .NET idioms and practices.

As part of the task you will need to generate a document explaining the changes you made, and how they relate to the original PR.

You need to run related tests to ensure everything is working as expected.
If tests are failing, you will need to fix them.
