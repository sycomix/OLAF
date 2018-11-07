﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLAF
{
    public class ImageArtifact : Artifact
    {
        #region Constructors
        public ImageArtifact(long id, Bitmap image) : base(id)
        {
            Image = image;
        }

        public ImageArtifact(FileArtifact artifact, Bitmap image) : base(artifact.Id)
        {
            FileArtifact = artifact;
            Image = image;
        }
        #endregion

        #region Properties
        public FileArtifact FileArtifact { get; }

        public Bitmap Image { get; }

        public bool HasBitmap => Image != null;

        public bool HasFile => FileArtifact != null;

        public Dictionary<ImageObjectKinds, List<Rectangle>> DetectedObjects { get; }
            = new Dictionary<ImageObjectKinds, List<Rectangle>>();

        
        public List<ArtifactCategory> Categories { get; } = new List<ArtifactCategory>();

        public bool IsAdultContent { get; set; }

        public double AdultContentScore { get; set; }

        public bool IsRacy { get; set; }

        public double RacyContentScore { get; set; }
        
        public List<string> OCRText { get; set; }

        public bool HasOCRText => OCRText != null;
        #endregion

        #region Methods
        public bool HasDetectedObjects(ImageObjectKinds kind) => this.DetectedObjects.ContainsKey(kind)
            && this.DetectedObjects[kind].Count > 0;

        public void AddDetectedObject(ImageObjectKinds kind, Rectangle r)
        {
            if(!DetectedObjects.ContainsKey(kind))
                    {
                DetectedObjects.Add(ImageObjectKinds.FaceCandidate, new List<Rectangle>() { r });
            }
            else
            {
                DetectedObjects[kind].Add(r);
            }
        }
        #endregion
    }
}