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

Our application is hosted Azures App Service where the component Chirp.Web handles the GUI and Chirp.Infrastructure handles the repositories and database model.
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

### Project board
All issues in the project backlog are solved/closed, which includes all the required features and additional features.
![ProjectBoard](https://hackmd.io/_uploads/BJmexpaI6.png)

#### Optimize profile-picture handling
In this version of Chirp, Author profile pictures are stored as a reference string to an image stored locally in the `wwwroot/images` folder. This means that you have to push your image to GitHub each time you change profile picture. We have therefore discussed that if we had more unpaid space in the database we would store the whole image under each Author, hence have a more scalable and secure solution.

### From issue to main
The following flowchart illustrates how group 18 creates issues (green box), develops solutions (red box), and merges the solutions into the `main` branch (purple box). The flowchart also shows how different decisions/statements are made through the flow, such as "Is the task understood".
![FromIssueToMain](https://hackmd.io/_uploads/BJKn42TUT.jpg)

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

You have to install PowerShell in order for the playwright test to run

#### Installing

##### Windows and Linux

```
cd test/Chirp.Web.Ui.Tests
pwsh bin/Debug/netX/playwright.ps1 install
```

##### Mac
We haven't found a way to run the playwright test on mac. If you want to look at the result of the tests, look on GitHub.

#### Running
go to the root of the project and run

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

For writing code, both LLMs were only helpful to a minor degree, for increasing the development speed and code readability when knowing what to prompt or via CoPilot auto-completion. However, in many cases it was faster to read the documentation and manually implement the code, especially when not knowing exactly what to prompt. Mainly, it was ChatGPT that had such cases as it only relies on the prompt and has no code base knowledge. CoPilots auto-completion also suggested old and incorrect code a few times, once again making it faster to manually write the code.

Both LLMs were a bit more helpful when debugging, as they in many cases were able to quickly give suggestions to fix the use and an explanation of the error without having to read through many long error stacks containing confusing commands and methods.

### CodeFactor

CodeFactor was used on each pull request, automatically checking the cleanliness and readability of the pushed code. If CodeFactor found any unclean or irregular code in the pull request, it would either issue or apply fixes.
