namespace AssLoader
{
    /// <summary>
    /// Describes how subtitles are moved to prevent onscreen collisions.
    /// </summary>
    public enum CollisionStyle
    {       
        /// <summary>
        /// SSA will attempt to position subtitles in the position specified by the "margins". 
        /// However, subtitles can be shifted vertically to prevent onscreen collisions. 
        /// With "normal" collision prevention, the subtitles will "stack up" one above the other,
        /// but they will always be positioned as close the vertical (bottom) margin as possible
        /// - filling in "gaps" in other subtitles if one large enough is available.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Subtitles will be shifted upwards to make room for subsequent overlapping subtitles.
        /// This means the subtitles can nearly always be read top-down,
        /// but it also means that the first subtitle can appear half way up the screen before the subsequent overlapping subtitles appear. 
        /// It can use alpha lot of screen area.
        /// </summary>
        Reverse
    }

    public enum BorderStyle
    {
        OutlineAndDropShadow = 1,
        OpaqueBox = 3
    }

    public enum AlignmentStyle
    {
        BottomLeft = 1,
        BottomCentered = 2,
        BottomRight = 3,
        MiddleLeft = 4,
        MiddleCentered = 5,
        MiddleRight = 6,
        TopLeft = 7,
        TopCentered = 8,
        TopRight = 9
    }

    /// <summary>
    /// The wrapping style. 
    /// </summary>
    public enum WrapStyle
    {
        /// <summary>
        /// smart wrapping, lines are evenly broken, only \N breaks.
        /// </summary>
        Average = 0,
        /// <summary>
        /// same as <see cref="Average"/>, but upper line gets wider.
        /// </summary>
        Downward = 1,
        /// <summary>
        /// no word wrapping, \n \N both breaks.
        /// </summary>
        None = 2,
        /// <summary>
        /// same as <see cref="Average"/>, but lower line gets wider.
        /// </summary>
        Upward = 3
    }

}