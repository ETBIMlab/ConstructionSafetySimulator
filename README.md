# Construction Safety Simulator - Git Guide

## First Things First!
Our project is currently built on Unity version **2019.3.1f1** (This version is no long supported to we will need to upgrade to 2019.3.15, will discuss at sprint meeting)
This is **really _important_**! If we don't all use the same version, then whenever someone pushes new code, merge conflicts are bound to happen more often. If you aren't sure which version you're using, please install the [Unity Hub](https://unity3d.com/get-unity/download) from this link. You can add a different version of unity by clicking "Installs" and then selecting the blue "ADD" button. 

![Image of Unity Hub](https://i.imgur.com/wfvQaG5.png)
![Image of project version](https://i.imgur.com/lG8ER8C.png)

## How Do I Use Git?
The easiest way to manage your git commands is through the terminal! I would **strongly** recommend dowloading [CMDR](https://cmder.net/) (Use "Download Full"). All you need to do is unzip it into a folder in your User directory. It's free and lightweight and it allows you to use Unix style commands instead of Windows commands. It also comes with git pre-installed. If you already have a terminal setup you're happy with, great! But this tutorial will us Unix terminal commands, so be ready! (Serously, CMDR is awesome)

**Here's a git cheat sheet! More explanation below**
```
git fetch                       // gets info from the main repository, you can use it to check for changes
git status                      // check the status of your current changes, see if you're up to date
git pull                        // update your local repo with the latest on the main repo (will not overwrite your work!)
git checkout -b myNewBranch     // creates a new branch and checks it out simultaneously!
git add -A                      // add all changes to the stage
git add filename.etc            // add a specific file only, to the stage
git commit -m "my message"      // simultaneously commit and add a commit message
git push                        // pushes your commit to the main repo
git checkout master             // switch back to the master branch 
```
**If you get stuck** and you just want to delete it all and start over you can use `git reset --hard` and that will revert you to the last commit and delete **all** of your changes since the last commit **without any confirmation** so use it wisely. Seriously, be careful with this command! (but don't worry, anthing commited is safe)


## Step 1: Cloning the repo!
It's pretty simple! Just create a folder somewhere in your user directory (or wherever you want to keep the project) and then in the terminal, navigate to that folder. Once you're in the folder that you want to clone to repo into, type the following command
```
git clone https://github.com/WillJenkins/ConstructionSafetySimulator.git
```
You will the be prompted to enter your GitHub username and password 

**A quick note about navigation using terminal commands**
Unfamiliar with terminal? Let's say you have the folder `C:\Users\MyName\ConstructionSim` and you want to put the project there. 
In your terminal, the command `cd` or "change directory" is how you move from folder to folder. 

If you're in the `Users` directory and you want to move into a folder `C:\Users\MyName\ConstructionSim`, type:
```
cd MyName           // it would be your user name
cd ConstructionSim
```
If you need to move back out of a folder, use
```
cd ../
```
If you need to see all the stuff in the current folder, type
```
ls
```

***If you're unfamiliar with terminal commands*** and you find this too intimidating, please contact Will Jenkins in Discord or wjenkin3@asu.edu. I'd be happy to help! 


### Once the repo has been cloned

The next step is to add the project to Unity. The best way to do this is to open Unity Hub, and under the "Projects" tab, select the ADD button, navigate to the folder `ConstructionSafetySimulator` and then select the folder ***inside*** that folder that has the same name. 

![Image of the correct folder](https://i.imgur.com/omgiMRL.png)

Once you select it, Unity Hub will allow you to open the project. It may prompt you to update the version. The first time you open the project it may take a while. This is because our GitHub version is not the entire Unity project. Unity has to fill in the missing info for your specific machine. 

***One Final Note***
When you first create the project and open it, you need to double-click on the Scene in order to see everything. It will default to a blank editor. 

## Ready To Edit Some Code?

************************** ***EVERYONE SHOULD READ THIS PART*** ******************************

***Before you open Unity*** 
You need to checkout a new branch! Please ***do not*** push changes directly to the `Master Branch`. 
You should checkout a new branch ***every time*** you make a change. 
And please, for the love of Alan Turing, **do not** keep one branch that you use over and over without merging. 
***Branches should be used for one or two changes!*** 

Luckily, checking out a new branch is easy! (`checkout` just means "change my repo into this branch") 
Before you do, though, make sure you are up to date with the latest. You should start on the `Master Branch` and update your repo
```
git checkout master   //not needed if you're already on master
git fetch
git pull
```

I also like to do a `git status` to make sure I don't have any unwanted changes lingering. Status will show you if there are any changes you have made that don't exist in the `Master` 

**Okay**, so you're up to date now! Next, come up with a name for your branch. Make it unique! Like "addingThumbadControls" or "modifyingSmokeAnimation". Make it something that isn't likely to be reused by someone else. 

Let's say I'm gonna make the branch "workingOnSomeCode" all you need to type is
```
git checkout -b workingOnSomeCode
```

The `-b` will create the branch and immediately switch you to it. 
You can also use 
``` 
git branch workingOnSomeCode
git checkout workingOnSomeCode
```
But using the `checkout -b` action is just less typing! 

That's it! Now you can open Unity!

***NOTE***
**If you forget to checkout a branch before you start editing** don't panic! Just save & close Unity and then checkout a new branch, same as above. Run a `git status` afterward to make sure you still see your changes. 

## Push That Code! 
So, you've been chuggin' away and now it's time to save your work? Great! 
There are three commands you need. `add` `commit` and `push`

Add everything you've been working on to the "stage"
```
git add -A
```
If you do `git status` at this point, you should see all your changes in green!

```
git commit -m "A message about my changes (limit 40 chars)"
```
and before you push, run `git pull` first to pull down any changes that may have happened before you do the next step:
```
git push       //pushes your changes to the GitHub repo
```

Now your changes are saved! But, you're still on your branch. You can walk away now and come back later to add more stuff, OR you can `merge` your changes to the `master` by creating a `pull request`! You need to do this step when you're done with your sprint task.

## Merging Your Code

Something we didn't do in the past that we need to start doing now it **testing** each other's code before merging to the master.
So, now you're done with your sprint task and you're ready to merge it in with the `master`? You need to go to the project's GitHub page!

![Image of github page](https://i.imgur.com/jjRTWAE.png)

Click on `branches` and then click `New Pull Request`
This will create a page where we can all see what changes were made and if there are any conflicts. 
***PLEASE DON'T MERGE YOUR OWN PULL REQUESTS WITHOUT REVIEWS.***
Put a message in the Discord that you have a PR and it needs to be tested. Let at least one person review it before you merge it. 
If you want to review someone's changes, click "Submit Review" and add any comments you want to add. Even if it's just a thumbs up emoji. (Yes GitHub supports emoji!)

**NOTE** If you only made a small change and you don't think it needs to be reviewed, you can merge your own PR, just please be responsible! We're all in this thing together! Good luck out there! 

# TL;DR

Start on `master`
```
git checkout master
git pull
```
checkout a new branch
```
git checkout -b myCoolNewBranch
```
Make some cool new stuff in the project, and then push it
```
git add -A
git commit -m "I did a thing!"
git push
```
then create a pull request on GitHub, wait for review, and merge that puppy!
