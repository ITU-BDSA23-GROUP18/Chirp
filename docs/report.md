---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2023 Group 18
author:
  - "Benjamin von Barner Altenburg <beal@itu.dk>"
  - "David Alexander Feldner <dafe@itu.dk>"
  - "Jack Kryger Sørensen <jkrs@itu.dk>"
  - "Mads Valentin Jensen <mvje@itu.dk>"
  - "Severin Ernst Bøgelund Madsen <sevm@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

![Class diagram showing the types used in the application and stored in the database.](https://hackmd.io/_uploads/ryOdgqTIT.png)

## Architecture — In the small
![Architecture.drawio (12)](https://hackmd.io/_uploads/HkMmf6aIp.png)



## Architecture of deployed application

Our application is hosted by Azures App Service where the component Chirp.Web handles the GUI and Chirp.Infrastructure handles the repositories and database model.
A user connects their machine to Chirp.Web through the Azure App Service. Reading or writing data will make Chirp.Web read from the repositories in Chirp.Infrastructure, that will call the Azure SQL Server database. When logging in, Chirp.Web calls GitHub Authentication.

![Component diagram showing the architecture of the deployed Chirp application.](https://hackmd.io/_uploads/HJeeXcTUp.png)

## User activities
![Possible of User activities](https://hackmd.io/_uploads/HyrLkj6Lp.png)

## Sequence of functionality/calls through _Chirp!_
![Sequence of calls](https://hackmd.io/_uploads/HJmGbqaIp.png)

# Process

## Build, test, release, and deployment
![GH-Actions-UML-Activity-Diagrams.drawio](https://hackmd.io/_uploads/Hkji4ca86.png)

## Teamwork

### Project Board
All issues in the project backlog are solved/closed, which includes the required features and additional features.
![ProjectBoard](https://hackmd.io/_uploads/BJmexpaI6.png)

#### Optimize profile-picture handling
In this version of Chirp, Author profile pictures are stored as a reference string to an image stored locally in the `wwwroot/images` folder. This means you must push your image to GitHub each time you change your profile picture. Therefore, We have discussed that if we had more unpaid space in the database, we would store the whole image under each Author, hence having a more scalable and secure solution.

### From issue to main
The following flowchart illustrates how group 18 creates issues (green box), develops solutions (red box), and merges the solutions into the `main` branch (purple box). The flowchart also shows how different decisions/statements are made through the flow, such as "Is the task understood".

![FromIssue2Main](https://hackmd.io/_uploads/rJTvtaT8a.jpg)

## How to make _Chirp!_ work locally
OBS! .Net 8 is required for running Chirp locally
Run the following commands:

```
Git clone https://github.com/ITU-BDSA23-GROUP18/Chirp.git
```

Go to the directory of the cloned repository (via `cd Chirp`) and run:

```
dotnet watch --project src/Chirp.Web
```

## How to run the test suite locally

### Our tests
We are testing our systems functionalities by the use of; **End-to-end tests, Integration tests, and Unit tests** for the different projects in our onion architecture.

#### Chirp.Infrastructure.Tests
The tests for Chirp.Infrastructure, test all the different repositories. This is done using a combination of unit and integration tests.

#### Chirp.Web.Tests/Chirp.Web.Ui.Tests
We use Playwright for our UI, which functions as our end-to-end tests. The tests go through our different features acting as a user would, assessing if the features work as intended. We also have some unit tests, testing the most basic things.

#### Chirp.Core.Tests
For the Chirp.Core, we have some simple tests for the DTO's, ensuring that their parameters can't be null.

### How to run the test

For the UI test to run, you must install Powershell if you are on a Linux system.

#### Windows and Linux

```
cd test/Chirp.Web.Ui.Tests
pwsh bin/Debug/netX/playwright.ps1 install
```

#### macOS (Apple)

*We have not found a way to successfully run the playwright test on macOS (Apple). If you want to see the results of the tests, look on GitHub.*

### Running the tests

Go to the root of the project and run.

```
dotnet test
```

# Ethics

## License

- The MIT license

## LLMs, ChatGPT, CoPilot, and others

### CoPilot and ChatGPT

CoPilot and ChatGPT were both used for:

- Code generation and auto-completion.
- Debugging and understanding errors.

For writing code, both LLMs were only helpful to a minor degree in increasing the development speed and code readability when knowing what to prompt or via CoPilot auto-completion. However, in many cases, it was faster to read the documentation and manually implement the code, especially when not knowing exactly what to prompt. Mainly, it was ChatGPT that had such cases as it only relies on the prompt and has no code base knowledge. CoPilots auto-completion also suggested old and incorrect code a few times, making it faster to manually write the function.

Both LLMs were a bit more helpful when debugging, as they in many cases were able to quickly give suggestions to fix the use and an explanation of the error without having to read through many long error stacks containing confusing commands and methods.

### CodeFactor

CodeFactor was used on each pull request, automatically checking the cleanliness and readability of the pushed code. If CodeFactor found any unclean or irregular code in the pull request, it would either issue or apply fixes.
