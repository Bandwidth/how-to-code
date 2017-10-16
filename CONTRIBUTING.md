# How to submit an issue

Before submitting an issue, make sure your question was not already answered by searching through the Bandwidth archive.

##Submit an Issue

If you have found a bug, open a new issue. Provide the following information about the bug:

* **Overview of the issue** - what errors are being thrown?
* **Use case** - explain what you are trying to do, why is this a bug?
* **Operating System** - is this a problem on all systems?
* **Give us the error** - provide code so we can see the error
* **Related issues** - have similar issues been reported?
* **Suggested fix** - what might be causing the issue if you cannot fix it yourself?

##Submitting a Pull Request

* Search through Bandwidth's open and closed pull request to ensure you are not submitting something that has already been submitted.
* Make all necessary changes in a new git branch:
    ```shell
    git checkout -b new-fixed-branch
    ```
* Make all necessary changes to the code
* Test your changes before committing the code
* Add the changes to be committed
    ```shell
    git add
    ```
* Commit the changes you have made with a descriptive commit message
    ```shell
    git commit -m "add description here"
    ```
* Push your changes to Github
    ```shell
    git push
    ```
* Open a pull request in Github

If we ask you to make any necessary changes:

* Make the required changes on your branch
* Test the changes to make sure the code works
* Add and commit the changes
* Push your changes to Github. This will automatically update the pull request.

After the your branch has been merged, please delete your branch.








