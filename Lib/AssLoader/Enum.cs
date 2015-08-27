namespace AssLoader
{
    /// <summary>
    /// Describes how subtitles are moved to prevent onscreen collisions.
    /// </summary>
    public enum CollisionStyle
    {       
        /// <summary>
        /// SSA will attempt to position subtitles in the position specified by the "margins". 
        /// </summary>
        /// <remarks>
        /// However, subtitles can be shifted vertically to prevent onscreen collisions. 
        /// With "normal" collision prevention, the subtitles will "stack up" one above the other,
        /// but they will always be positioned as close the vertical (bottom) margin as possible
        /// - filling in "gaps" in other subtitles if one large enough is available.
        /// </remarks>
        Normal = 0,
        /// <summary>
        /// Subtitles will be shifted upwards to make room for subsequent overlapping subtitles.
        /// </summary>
        /// <remarks>
        /// This means the subtitles can nearly always be read top-down,
        /// but it also means that the first subtitle can appear half way up the screen before the subsequent overlapping subtitles appear. 
        /// It can use a lot of screen area.
        /// </remarks>
        Reverse = 1
    }

    /// <summary>
    /// The border style of subtitles.
    /// </summary>
    public enum BorderStyle
    {
        /// <summary>
        /// Use outline and drop shadow.
        /// </summary>
        OutlineAndDropShadow = 1,
        /// <summary>
        /// Use an opaque background bounding box.
        /// </summary>
        OpaqueBox = 3
    }

    /// <summary>
    /// Describes the alignment of subtitles.
    /// </summary>
    public enum AlignmentStyle
    {
        ///<summary>The subtitle or the archor point will be set at bottom left.</summary>
        BottomLeft = 1,
        ///<summary>The subtitle or the archor point will be set at bottom center.</summary>
        BottomCenter = 2,
        ///<summary>The subtitle or the archor point will be set at bottom right.</summary>
        BottomRight = 3,
        ///<summary>The subtitle or the archor point will be set at middle left.</summary>
        MiddleLeft = 4,
        ///<summary>The subtitle or the archor point will be set at middle center.</summary>
        MiddleCenter = 5,
        ///<summary>The subtitle or the archor point will be set at middle right.</summary>
        MiddleRight = 6,
        ///<summary>The subtitle or the archor point will be set at top left.</summary>
        TopLeft = 7,
        ///<summary>The subtitle or the archor point will be set at top center.</summary>
        TopCenter = 8,
        ///<summary>The subtitle or the archor point will be set at top right.</summary>
        TopRight = 9
    }

    /// <summary>
    /// The wrapping style of subtitles.
    /// </summary>
    public enum WrapStyle
    {
        /// <summary>
        /// Smart wrapping, lines are evenly broken, only @"\N" breaks.
        /// </summary>
        Smart = 0,
        /// <summary>
        /// End-of-line word wrapping, only @"\N" breaks.
        /// </summary>
        EndOfLine = 1,
        /// <summary>
        /// No word wrapping, both @"\n" and @"\N" breaks.
        /// </summary>
        None = 2,
        /// <summary>
        /// Same as <see cref="Smart"/>, but lower line gets wider.
        /// </summary>
        Smart2 = 3
    }

}