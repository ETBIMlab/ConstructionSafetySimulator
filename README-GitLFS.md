# Git LFS, AKA Git L)arge F)ile S)torage

## What is it: _Get for >100MB files_
Git LFS is an external file storage solution used to divert exceedingly large files of your choosing into the LFS server. 
After storing the files, it replaces the files in your repo with file pointers. 
This means that these large files no longer reside in the repo, but instead in the LFS server. 
These large files are still integrated seamlessly when a git clone is performed.
https://git-lfs.github.com/

## Why is it Needed: _Standard Git disallows >100MB files_
Github has a limit to how large your files can be when committing. 
Any file larger than 50MB will start giving warnings when pushed, and files larger than 100MB will be blocked from pushing entirely. 
Git LFS has no limit to the size of the file you can commit. 

## Pros and Cons
Pros: 
Avoids github file size limit, as some files are approaching maximum size
Stores up to 1GB + 1GB/month for free
Free to install

Cons:
May have to pay for it, the free data plan of 1GB a month may not be enough
It costs $5 a month for the 50GB data plan, see link for details on other pricing options.
https://docs.github.com/en/billing/managing-billing-for-git-large-file-storage/viewing-your-git-large-file-storage-usage
Once the repo has been converted, everyone must install git LFS otherwise, they can't do any work

## Install in 2 Steps
1. Download Here: https://git-lfs.github.com/
2. run `git lfs install`

Q: How do I know it got installed?
A: run `git config -l --show-origin | grep lfs`
   After running that command, you should see an output like this:
   ```
   file:C:/Users/<USER>/.gitconfig  filter.lfs.clean=git-lfs clean -- %f
   file:C:/Users/<USER>/.gitconfig  filter.lfs.smudge=git-lfs smudge -- %f
   file:C:/Users/<USER>/.gitconfig  filter.lfs.process=git-lfs filter-process
   file:C:/Users/<USER>/.gitconfig  filter.lfs.required=true
   file:.git/config        lfs.repositoryformatversion=0
   ```

## Usage for Team Members: _No change in workflow_
Once git lfs has been installed and the large files in the repo have been migrated to git LFS, it works in the exact same way as before.
Git LFS is transparent, meaning the process of adding, committing, pushing and pulling all work the same.

## Usage for Git Admin
```
# Example: make all prefab files reside on LFS Server
#
# Sending .prefab's to LFS...
$ git checkout -b <USER>/<BRANCH>

# First: Make new .prefab files go to LFS (modifies .gitattributes!)
$ git lfs track "*.prefab"

# Next: Make existing .prefab files go to LFS
$ git lfs migrate import --include="*.prefab"

# Finally: Commit/push LFS changes to repo...
$ git add --renormalize . # Go through every file in repo and re-add using file pointer
$ git commit -m "move .prefab files to LFS"
$ git push origin mnfitz/morelfs

# PullRequest here!
```
## Recommended plan for migrating: 
Dummy commits to `ETBIM-Research-Group/LFS_Test`
All users perform `git lfs install`
Then pick obscure filetype (ex: .psd), and migrate it to git LFS
Evaluate usage over week
Pick another less obscure filetype (ex: .fbx), and migrate to git lfs
Evaluate again over one week