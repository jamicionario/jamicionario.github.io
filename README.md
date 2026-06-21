
# Jamictionary

This technical README explains how to run the project locally, and how to deploy it.

See also [the README file of the ScoresProcessor](ScoresProcessor/README.md) sub-project.

Contents:

- [Deploying](#deploying)
- [Running locally](#running-locally)
  - [Running from console](#running-from-console)
  - [Running from VS Code](#running-from-vs-code)
- [Installing](#installing)
  - [MacOS](#macos)
- [Code scaffolding](#code-scaffolding)

## Deploying

When data is updated, it's necessary to update and deploy:

1. Update data with the Scores Processor — see the [README](ScoresProcessor/README.md) file there.
2. Check the website [locally on your machine](#running-locally), and verify that the website and the data are OK.
3. Then deploy with:  
   `ng deploy`

Note that the deploy process publishes exactly what you have locally, **even changes that are not comitted**.

For more information, see the [angular-cli-ghpages documentation](https://www.npmjs.com/package/angular-cli-ghpages).

## Running locally

You can run the Jamictionary on your local machine.  
It can be run from the console or from VS Code.

First, you will need to:

- Install the needed tools — see the section "[Installing](#installing)" below.
- Generate the data using the Scores Processor — check the information on [its README](ScoresProcessor/README.md) file.

Then you can run it with the instructions that follow.

Once the server is running, open your browser and navigate to <http://localhost:4200/>.  
The application automatically reloads whenever you modify any of the source files.

### Running from console

To run the Jamictionary from console:

- Open a terminal;
- run with `ng serve`

### Running from VS Code

To run or debug the Jamictionary from VS Code:

  1. Select the "Run and Debug" tab on the left;
  2. Choose the option "ng serve" on the dropdown.
  3. Start debugging with the green ▶️ "Run" button.

![screenshot of running in VS Code from the root folder](docs/processing%20data%20from%20VS%20Code.png)

## Installing

These are instructions for your one-time setup.

1. Install git.
2. Install Angular following the instructions below according to your <abbr title="Operating System">OS</abbr>.
3. Install angular-cli-ghpages: `npm install -g angular-cli-ghpages`

### MacOS

Install Angular:

1. Install [HomeBrew](https://brew.sh/):
    `/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"`

2. Install node via brew:
    `brew install node`
   1. To update: `brew upgrade node`
   2. To choose a specific LTS version: `brew install node@24`

3. Install Angular:
    `npm install -g @angular/cli`

You can see the NPM installed packages with `npm list -g`

## Code scaffolding

Angular CLI has good code scaffolding tools.

For example, to generate a new component run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```
