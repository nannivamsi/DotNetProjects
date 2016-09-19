using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitApiHelper
{
    class Program
    {
        private const string RemoteName = "Demo";// Default repository name.
        static string path = @"C:\HPE HUB\Temp Clone"; // Current release 'git' folder.
        static string originBranch = "origin/v2.0"; // Branch.
        static string currentTag = "v1.8";// Current tag version which is existing to be cloned for deployment.
        static string newAddedTag = "v1.9.4";// New tag version for which need to be deployed.
        static string originalRepoPath = "https://github.com/nannivamsi/Demo"; // Current repository URL.
        private static string currentTagRference = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                #region Initial Code commented
                //ClenaRepoDirectory(path);  
                //string clonedRepoPath = Repository.Clone(originalRepoPath, path);
                //CanForcefullyCheckoutWithConflictingStagedChanges(currentTag);
                ////CanFetchAllTagsIntoAnEmptyRepository(originalRepoPath);
                //CanAddALightWeightTagFromSha();
                //CanForcefullyCheckoutWithConflictingStagedChanges(newAddedTag);
                ////CanPushABranchTrackingAnUpstreamBranch();
                //PushVersion(); 
                #endregion
                try
                {
                    // Repos Changes 2
                    Repository.Clone(originalRepoPath, path);
                    CanFetchAllTagsIntoAnEmptyRepository(originalRepoPath, true);
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2146233088)
                        CanFetchAllTagsIntoAnEmptyRepository(originalRepoPath, false);
                    else
                        throw;
                }
                CanForcefullyCheckoutWithConflictingStagedChanges(currentTag);
                CanAddALightWeightTagFromSha();
                CanForcefullyCheckoutWithConflictingStagedChanges(newAddedTag);
                PushApi();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }


        #region Working Code
        //TODO: All this methods can be consolidated to single method.
        static void CanFetchAllTagsIntoAnEmptyRepository(string url, bool firstSetup)
        {


            using (var repo = new Repository(path))
            {
                Tag currentRefTag = repo.Tags[currentTag];
                if (firstSetup)
                    repo.Network.Remotes.Add(RemoteName, url);
                string refSpec = string.Format("refs/tags/{2}:" + currentRefTag.CanonicalName, RemoteName, currentTag, currentTag);
                // Perform the actual fetch
                Commands.Fetch(repo, RemoteName, new string[] { refSpec }, new FetchOptions
                {
                    TagFetchMode = TagFetchMode.Auto
                }, null);
                Console.WriteLine("Fetch passed with version {0} ", currentTag);
            }
        }

        public static void CanForcefullyCheckoutWithConflictingStagedChanges(string tag)
        {
            using (var repo = new Repository(path))
            {
                Branch currentBranch = repo.Branches[originBranch];
                Branch currentRepoBranchTag = Commands.Checkout(repo, tag);
                currentTagRference = currentRepoBranchTag.CanonicalName;
                Console.WriteLine("Branch switched to {0} version", tag);
            }
        }

        public static void CanAddALightWeightTagFromSha()
        {
            using (var repo = new Repository(path))
            {
                Tag currentRefTag = repo.Tags[currentTag];
                Tag newTag = repo.Tags.Add(newAddedTag, currentRefTag.Reference.CanonicalName);
                Console.WriteLine("New Tag created with  {0} version", newAddedTag);
            }
        }

        private static void PushApi()
        {
            using (var repo = new Repository(path))
            {
                Remote remote = repo.Network.Remotes["origin"];
                var options = new PushOptions();
                var refTag = @"refs/tags/" + newAddedTag;
                options.CredentialsProvider = (url, user, cred) =>
                    new UsernamePasswordCredentials { Username = "nannivamsi", Password = "Vamsi@123" };
                repo.Network.Push(remote, refTag, options);
                Console.WriteLine(" {0} version Code is now available in Remote server", newAddedTag);
            }
        }

        #endregion



        #region Clean Directory, Commented code
        //private static void ClenaRepoDirectory(string repoDirpath)
        //{
        //    System.IO.DirectoryInfo di = new DirectoryInfo(repoDirpath);

        //    foreach (FileInfo file in di.GetFiles())
        //        file.Delete();

        //    foreach (DirectoryInfo dir in di.GetDirectories())
        //        dir.Delete(true);

        //} 
        #endregion

        #region Pull code commented
        //public static void CanPullIntoEmptyRepo()
        //{
        //    string url = "https://github.com/libgit2/TestGitRepository";
        //    string remoteName = "origin";
        //    string repoPath = @"C:\HPE HUB\Temp Clone";
        //    Identity Identity = new Identity("Vamsi", "nanni.vamsi@hotmail.com");
        //    Signature Signature = new Signature(Identity, new DateTimeOffset(2016, 06, 16, 10, 58, 27, TimeSpan.FromHours(2)));
        //    using (var repo = new Repository(repoPath))
        //    {
        //        // Set up remote
        //        repo.Network.Remotes.Add(remoteName, url);

        //        // Set up tracking information
        //        repo.Branches.Update(repo.Head,
        //            b => b.Remote = remoteName,
        //            b => b.UpstreamBranch = "refs/heads/v2.0");

        //        // Pull!
        //        MergeResult mergeResult = Commands.Pull(repo, Signature, new PullOptions());


        //    }
        //} 
        #endregion

        #region Push API non working Code
        //public static void CanPushABranchTrackingAnUpstreamBranch()
        //{
        //    bool packBuilderCalled = false;
        //    PackBuilderProgressHandler packBuilderCb = (x, y, z) => { packBuilderCalled = true; return true; };


        //    var options = new PushOptions() { CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials { Username = "nannivamsi", Password = "Vamsi@123" } };
        //    string pushRefSpec = @"refs/heads/master";

        //    //AssertPush(repo => repo.Network.Push(repo.Network.Remotes["origin"], "Tag", newTagRference, options));
        //    using (var repo = new Repository(path))
        //    {
        //        repo.Network.Push(repo.Network.Remotes["origin"], pushRefSpec, "master", options);
        //    }
        //}

        //private static void OnPushStatusError(PushStatusError pushStatusErrors)
        //{
        //    var format = string.Format("Failed to update reference '{0}': {1}", pushStatusErrors.Reference, pushStatusErrors.Message);
        //}

        //private static void AssertPush(Action<IRepository> push)
        //{
        //    string clonedRepoPath = Repository.Clone(originalRepoPath, path);

        //    using (var originalRepo = new Repository(originalRepoPath))
        //    using (var clonedRepo = new Repository(clonedRepoPath))
        //    {
        //        Remote remote = clonedRepo.Network.Remotes["origin"];

        //        // Compare before
        //        //if(originalRepo.Refs["Tags"].ResolveToDirectReference().TargetIdentifier==
        //        //             clonedRepo.Refs["Tags"].ResolveToDirectReference().TargetIdentifier);
        //        //Assert.Equal(
        //        //    clonedRepo.Network.ListReferences(remote).Single(r => r.CanonicalName == "refs/heads/master"),
        //        //    clonedRepo.Refs.Head.ResolveToDirectReference());

        //        // Push the change upstream (remote state is supposed to change)
        //        push(clonedRepo);

        //        // Assert that both local and remote repos are in sync
        //        //Assert.Equal(originalRepo.Refs["HEAD"].ResolveToDirectReference().TargetIdentifier,
        //        //             clonedRepo.Refs["HEAD"].ResolveToDirectReference().TargetIdentifier);
        //        //Assert.Equal(
        //        //    clonedRepo.Network.ListReferences(remote).Single(r => r.CanonicalName == "refs/heads/master"),
        //        //    clonedRepo.Refs.Head.ResolveToDirectReference());
        //    }
        //} 
        #endregion

        #region Push API Command line as argument code
        //private static void PushVersion()
        //{
        //    string arguments = "push -u origin " + newAddedTag;

        //    ProcessStartInfo startInfo = new ProcessStartInfo("git.exe")
        //    {
        //        UseShellExecute = false,
        //        WorkingDirectory = path,
        //        RedirectStandardInput = true,
        //        RedirectStandardOutput = true
        //    };

        //    startInfo.Arguments = arguments;
        //    Process process = new Process();
        //    process.StartInfo = startInfo;
        //    process.Start();

        //    List<string> output = new List<string>();
        //    string lineVal = process.StandardOutput.ReadLine();

        //    while (lineVal != null)
        //    {
        //        Console.WriteLine(lineVal);
        //        output.Add(lineVal);
        //        lineVal = process.StandardOutput.ReadLine();

        //    }
        //} 
        #endregion
    }
}
