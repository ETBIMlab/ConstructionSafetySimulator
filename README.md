# ConstructionSafetySimulator

<strong>Instructions for setting up repository</strong>

The easiest way to manage git projects if you're unfamiliar with git, or you just want something simple, is to download GitHub desktop and sign in with your GitHub account. This is what I'm using for this project and it's great. Highly recommended. 

Another important tool: Unity Hub. It makes it really easy to manage your unity projects. 

<strong>Step 1: Cloning The Repo</strong>
In GitHub desktop, select the option to clone a repository, use this link:<br />
https://github.com/WillJenkins/ConstructionSafetySimulator.git <br />
make sure and put it in a folder that's easy for you to find. 

This step will copy all the contents of the repo to your machine, but it is not a complete Unity project yet. 
Unity projects contain a lot of machine specific files that will need to be added in by your machine.

<strong>Step 2: Adding The Project To Unity</strong><br />
<a href="https://unity3d.com/get-unity/download"> Download Unity and Unity Hub here</a>

Open Unity Hub. Click on "ADD" in the top right corner. Navigate to the cloned repository you just made and select 
the folder "ConstructionSafetySimulator" that's INSIDE the parent folder. In other words, you want to select the folder 
called "ConstructionSafetySimulator" that's in the same folder as README.md, gitignore, etc.


Now you should see the project in your Unity Hub. Once you open it, it should fill in the remaining components of the project. <br />
<strong>Important:</strong> in order to view the scene the first time you create the project you must open the "scene" called "sampleScene" in Unity.

<strong>Committing Changes To GitHub</strong><br />
If you've added stuff to the project and you're ready to push it to the master branch (be sure and save your project in Unity first), in GitHub desktop select the repo for the project, add text to the "summary" describing what changes you made, click commit, and then in the top right "push to origin". You may need to do a pull first, if someone else has pushed new changes since the last time you pulled from the repo. "pull" or "fetch" will update your repo with any new content that's been pushed since your last pull. This might get tricky, so we may need to experiment with using branches. 

<h2>Links To Helpful Videos</h2>

<a href="https://www.youtube.com/watch?v=iJ0oNYIUFJo">Setting up VR in Unity, Teleport, Object Interaction</a>
