# Overview

Welcome to the Github repo for the Pathfinder 2E level finder.
This project aims to use machine learning to estimate the level of a Pathfinder 2nd edition creature from some or all of its component stats.
This README will provide a basic explanation of how to access the app, as well as how to download and install it if you would like to make run it locally.
Additionally, it will provide a brief overview of the code with links for convenient browsing of the important parts of the codebase.
For a more detailed overview of the motivations, design principles, implementation, and empirical testing of the system, please reference the included [doc PDF](/P2ELevelFinderDoc.pdf)

# How to Use

The easiest way to try out the app for yourself is just to visit [this site](https://whatsmylevelp2e.netlify.app)

The app is deployed live there for the foreseeable future, and you can use the real fully functioning version of the application in the browser.

_Note that the site was designed for Chrome on a standard HD 1920x1080 monitor. It may function on other browsers or viewpower dimensions, but it also may have unforseen issues._

# How to Download/Install

The following process will allow you to download and install the code to run it locally if you so desire.

- Download or clone this repo
- Open the included .sln file in Visual Studio 2022. You will need .NET 6.0 installed, as well as the ASP .NET workload installed for VS.
- Run through Visual Studio, which will open up the application in your default browser and run it.

# Code Walkthrough

This section is designed to provide a basic overview of the core components of the code, and to provide useful links to make poking around in the codebase easier. If you would like a more detailed explanation of the design principles of the project, or of the data flow and operation of the code, reference the PDF design doc.

- [monsters.json](/HomebrewHelper/wwwroot/data/monsters.json): This JSON file contains the raw data used to construct the KNN cloud. This data contains all kinds of information on about 2000 monsters from various Pathfinder 2E source books. Credit to the user Glordrum on the Pathfinder 2E subreddit who scraped this data, and to the website [AonPRD](https://www.aonprd.com) from which the raw data was sourced.
- [RawMonsterRecord](/HomebrewHelper/Source/RawMonsterRecord.cs): This class is meant to be a 1:1 reflection of the important fields from the JSON data as a C# class. Because it directly mirrors the JSON, I'm able to query the web-server for the JSON and deserialize into an array of this object type with a single method call, greatly simplifying the data loading process.
- [Monster](/HomebrewHelper/Source/Monster.cs): This class is what represents a single point in the KNN cloud. It contains a variety of context information such as the monster's level, name, and description, as well as their n-dimensional position in the point cloud. Notably, this class has a function that allows them to be initialized directly from a RawMonsterRecord, making it easy to map from one to another during model initialization.
- [DataLoader Singleton](/HomebrewHelper/Source/DataLoaderSingleton/): This class/interface pair defines an injectable singleton service that allows Blazor components to load the JSON data directly into the KNN, or to load the test data and retrieve it as a list of Monsters. It is responsible for the data flow from the point of initialization to the point where the KNN model is completely built.
-[KNNCloud Singleton](/HomebrewHelper/Source/KNNCloudSingleton/): This class/interface pair defines an injectable singleton service that allows components to interact with the KNN model. It provides the utilities for setting the weights on the model, adding points to the cloud, and querying the model for both estimations, and for raw lists of neighbors.
- [Index.razor.cs](/HomebrewHelper/Pages/Index.razor.cs): While this file is technically part of the view code, and not the core functionality, it serves as the main entrypoint for the program, and therefore serves the purpose of a "main" function. Therefore, this file contains almost all of the code that triggers changes in the UI output, reacts to user input, and kicks off the initialization of the KNN model on load. It also is where the test code is which, when enabled, runs various analysis tests on the model and prints accuracy statistics and data to the console.

In general terms, the flow of the data during initialization goes:

- Index.razor.cs.OnAfterRenderAsync() > DataLoader.LoadData() > Monster.FromRawMonsterRecord() > KNNManger.AddPoint() _(repeating until all points loaded)_

And the flow for a single request made by the UI goes

- Index.razor.cs.RefreshQuery() > KNNManger.EstimateLevel() && KNNManager.GetNearestNeighbors()

Hopefully this information is helpful in reviewing the code and understanding what is going on, and don't forget to check out the live demo online, as it will be much easier than trying to run the system locally.