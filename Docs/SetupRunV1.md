# Setup and Run Umbraco with aspire V1

This is an easy guide to run the application, umbraco along with aspire. 

## Running Umbraco
* Start building the application by running ``Dotnet build`` in the ``CMS.Umbraco`` folder.
* The run the server from the same folder by saying ``Dotnet run`` this will run 2 seperate localhosts:
    1. https://localhost:44355/
    2. http://localhost:47176/

    Both localhosts shows the content that is being made, however to enter the Umbraco.CMS is an ``https`` url needed as: 
    * https://localhost:44355/umbraco


## Running Aspire

Run aspire by first enter the ``CMS.Umbraco.AppHost`` folder to run ``dotnet build`` followed by ``dotnet run``. 

Here you can administer the application from:
* https://localhost:17299/

However you'll need to log-in which is done by inserting the token, which is outputted in the terminal. It looks like this:

    [info:] Aspire.Hosting.DistributedApplication[0]
    Login to the dashboard at https://localhost:17299/logint=8ff7d41cf2b32ea69b93dac1b3fc2fe6

