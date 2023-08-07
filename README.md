# ContentProtector
 
An Umbraco 10+ package that prevents users from performing certain actions on content nodes selected in the backoffice.

Actions include saving, publishing, unpublishing, renaming, moving, copying, rolling back, deleting and trashing.

## TL;DR

1. install the plugin using the package manager `Install-Package ContentProtector`
2. launch your site and navigate to the backoffice settings section
3. select `Content Protector` from the the left hand section navigation in the `Third Party` area

From there you can set select actions and content nodes to protect.

## Known Issues

Due to the core setup, the 'copy', 'move' and 'trash' actions won't necessarily display the warning... However the action will have been prevented.

## Building and running the source

After forking and cloning the repo, open the Visual Studio solution and run the `ContentProtector.Web` project and navigate to the backoffice (a SQLite database is provided with a base set up).

Username: admin  
(Yes the username should be an email address... but we didn't want a dummy email address getting spammed with password reset requests!)  
Password: testtest1234

### Plugin Build

The `src\Gulp\gulpfile.js` file has a simple watch function that copies files from the `ContentProtector.Plugin/App_Plugins/` project to the `ContentProtector.Web/App_Plugins/` project when a file in the `ContentProtector.Plugin/App_Plugins/` path is saved.

To run the gulp task ensure you have gulp installed, open a command line, navigate to the `src\Gulp` folder in the location you've checked the repo out to, type gulp and hit return.

### Nuget Package Build

Use the `src\BuildPackage\build-package.ps1` powershell script to build the nuget packages (you'll need to place a `NuGet.exe` in the same directory if you don't have NuGet set up globally). This optionally outputs the generated packages to a local package source for testing.
