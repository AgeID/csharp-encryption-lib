# AgeID Encryption Helper for .NET #

A Visual Studio solution that allows building/updating the Class Libraries (one for .NET Framework and one for .NET Standard) that allow encrypting and decrypting AgeID payloads.



## Projects in solution ##
- AgeID.EncryptionHelper.Standard: The library source for .NET Standard
- AgeID.EncryptionHelper.Framework: The library source for .NET Framework
- AgeID.EncryptionHelper.Tests: MS Test Project with unit tests


## Building an updated NuGet package for .NET Framework ##
- Update the Assembly Information for the AgeID.EncryptionHelper.Framework project (Properties -> Application -> Assembly Information). You must at least update the version before publishing a new package.
- Go to the AgeID.EncryptionHelper.Framework project folder and open "AgeID.EncryptionHelper.Framework.nuspec" (the manifest) file with a text editor. Update any of the fields if applicable.
- Open a command prompt window in the same folder and run the command "nuget pack", which will generate a file in the current folder with the following name format {identifier}-{version}.nupkg.

Additional info can be found at https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio-net-framework


## Building an updated NuGet package for .NET Standard ##
- Right click on the AgeID.EncryptionHelper.Standard project and select "Properties", then select the "Package" tab (You need at least VS 2017)
- Review and update any fields as necessary. You MUST increment the version, at the least before publishing a new package.
- Right click on the project again, and select "Pack". Visual Studio builds the project and creates the .nupkg file. Examine the Output window for details which contains the path to the package file. Usually it's in \bin\Release\

Additional info can be found at https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio