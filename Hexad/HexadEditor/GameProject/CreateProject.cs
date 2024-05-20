using HexadEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HexadEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
    }

    class CreateProject : ViewModelBase
    {
        // TODO: use a better path from the installation location
        private readonly string _templatePath = @"..\..\HexadEditor\ProjectTemplates";

        private string _projectName = "NewProject";
        public string ProjectName 
        { 
            get { return _projectName; } 
            set
            {
                if(_projectName != value)
                {
                    _projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\HexadProjects\";
        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        /// <summary>
        /// Indicates whether a name or path is valid
        /// </summary>
        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (value != _isValid)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        /// <summary>
        /// Message to display to user onError
        /// </summary>
        private string _errorMsg;
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ObservableCollection<ProjectTemplate> ProjectTemplates { get { return _projectTemplates; } }
        //public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates
        //{ get; }

        /// <summary>
        /// Checks whether the values in projectName and projectPath are valid
        /// </summary>
        /// <returns></returns>
        private bool ValidateProjectPath()
        {
            string path = GetPathWithNamedEnding();

            IsValid = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Project cannot have empty name field.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) in project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Must assign project file location.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) in project path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected project folder already exists and is not empty.";
            }
            else // Name and Path are valid!
            {
                IsValid = true;
                ErrorMsg = string.Empty;
            }

            return IsValid;
        }

        /// <summary>
        /// Create a Hexad Project; returns a path to the project root file
        /// </summary>
        public string CreateNewProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if (!IsValid)
            {
                return string.Empty;
            }

            string path = GetPathWithNamedEnding();

            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder))); // creates subfolders in game project folder
                }

                // Hide files not relevant to the user such as template icons and screenshots
                var dirInfo = new DirectoryInfo(path + @".Hexad\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                var project = new Project(ProjectName, path);
                Serializer.ToFile(project, path + ProjectName + Project.Extension);

                return path;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error

                return string.Empty;
            }
        }

        /// <summary>
        /// Checks the end of a file path for proper characters, adds additional characters if necessary and appends the project name
        /// </summary>
        /// <returns></returns>
        private string GetPathWithNamedEnding()
        {
            string path = ProjectPath;
            if (!path.EndsWith(@"\")) path += @"\";
            path += $@"{ProjectName}\";
            return path;
        }

        /// <summary>
        /// Construction assembles Project Templates in CreateProjectView
        /// </summary>
        public CreateProject()
        {
            try
            {
                var templatesFiles = Directory.GetFiles(_templatePath, "template.json", SearchOption.AllDirectories);
                Debug.Assert(templatesFiles.Any());
                foreach (var file in templatesFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);

                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplates.Add(template);
                }
                ValidateProjectPath();
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error
            }
        }
    }
}
