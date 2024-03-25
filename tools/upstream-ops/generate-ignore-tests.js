const fs = require("fs");
const path = require("path");

// Load directory path from the PUPPETEER_TESTS environment variable
const directoryPath = process.env.PUPPETEER_TESTS;

// Function to generate JSON entry for each file
function generateJsonEntry(filename) {
    return {
        comment: "This is part of organizing the webdriver bidi implementation, We will remove it one by one",
        testIdPattern: `[${filename}] *`,
        platforms: ["darwin", "linux", "win32"],
        parameters: ["webDriverBiDi"],
        expectations: ["FAIL"],
    };
}

// Read directory
fs.readdir(directoryPath, (err, files) => {
    if (err) {
        return console.log("Unable to scan directory: " + err);
    }

    // Handle each file
    const entries = []
    files.forEach((file) => {
        const filename = path.parse(file).name;
        entries.push(generateJsonEntry(filename));
    });

    console.log(JSON.stringify(entries, null, 2));
});
