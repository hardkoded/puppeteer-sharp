const fs = require("fs");
const path = require("path");
const axios = require("axios");
require("dotenv").config();

const puppeteerSharpRepo =
    "https://api.github.com/repos/hardkoded/puppeteer-sharp/issues";
const repo ='hardkoded/puppeteer-sharp';

// Gets the token from the environment variables
const token = process.env.GITHUB_TOKEN;
const directoryPath = process.env.CDP_DIRECTORY;

fs.readdir(directoryPath, (err, files) => {
    if (err) {
        return console.log("Unable to scan directory: " + err);
    }

    // Handle each file
    files.forEach((file) => {
        const filename = path.parse(file).name;

        const title = `Split ${filename} class`;
        const body = `Get the ${filename} class ready for the bidi protocol`;
/*
        console.log(`Creating issue for ${filename}`);
        console.log(`Title: ${title}`);
        console.log(`Body: ${body}`);
*/
        // Check if the issue already exists
        axios
            .get(
                `https://api.github.com/search/issues?q=${title}+in:title+is:issue+repo:${repo}`,
                {
                    headers: {
                        Authorization: `token ${token}`,
                    },
                }
            )
            .then((response) => {
                const issues = response.data.items;
                if (issues.length > 0) {
                    console.log(`Issue found: ${title}`);
                } else {
                    console.log(`No issue found with the title: ${title}`);
                    return axios.post(
                        puppeteerSharpRepo,
                        {
                            title: title,
                            body: body,
                        },
                        {
                            headers: {
                                Authorization: `token ${token}`,
                            },
                        }
                    );
                }
            })
            .catch((error) => {
                console.error(error.message);
            });
    });
});
