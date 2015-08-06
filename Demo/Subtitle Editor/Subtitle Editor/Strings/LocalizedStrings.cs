using Windows.ApplicationModel.Resources;

namespace SubtitleEditor
{    
    public static class LocalizedStrings
    {
        private static readonly ResourceLoader loader = new ResourceLoader();

        public static string GetString(string resourceKey) => loader.GetString(resourceKey);

        /// <summary>
        /// Opportunity
        /// </summary>
        public static string AppAuthor
        {
            get;
        } = loader.GetString("AppAuthor"); 

        /// <summary>
        /// 一个字幕编辑软件。
        /// </summary>
        public static string AppDescription
        {
            get;
        } = loader.GetString("AppDescription"); 

        /// <summary>
        /// Subtitle Editor
        /// </summary>
        public static string AppName
        {
            get;
        } = loader.GetString("AppName"); 

        /// <summary>
        /// ASS 字幕文件
        /// </summary>
        public static string AssFileName
        {
            get;
        } = loader.GetString("AssFileName"); 

        /// <summary>
        /// 暗
        /// </summary>
        public static string ElementThemeDark
        {
            get;
        } = loader.GetString("ElementThemeDark"); 

        /// <summary>
        /// 默认
        /// </summary>
        public static string ElementThemeDefault
        {
            get;
        } = loader.GetString("ElementThemeDefault"); 

        /// <summary>
        /// 亮
        /// </summary>
        public static string ElementThemeLight
        {
            get;
        } = loader.GetString("ElementThemeLight"); 

        /// <summary>
        /// 取消
        /// </summary>
        public static string SaveDialogCancel
        {
            get;
        } = loader.GetString("SaveDialogCancel"); 

        /// <summary>
        /// 是否需要保存对当前文档的更改？
        /// </summary>
        public static string SaveDialogContent
        {
            get;
        } = loader.GetString("SaveDialogContent"); 

        /// <summary>
        /// 不保存
        /// </summary>
        public static string SaveDialogNo
        {
            get;
        } = loader.GetString("SaveDialogNo"); 

        /// <summary>
        /// 未保存的更改
        /// </summary>
        public static string SaveDialogTitle
        {
            get;
        } = loader.GetString("SaveDialogTitle"); 

        /// <summary>
        /// 保存
        /// </summary>
        public static string SaveDialogYes
        {
            get;
        } = loader.GetString("SaveDialogYes"); 

        /// <summary>
        /// 新建
        /// </summary>
        public static string SplitViewButtonNew
        {
            get;
        } = loader.GetString("SplitViewButtonNew"); 

        /// <summary>
        /// 打开
        /// </summary>
        public static string SplitViewButtonOpen
        {
            get;
        } = loader.GetString("SplitViewButtonOpen"); 

        /// <summary>
        /// 保存
        /// </summary>
        public static string SplitViewButtonSave
        {
            get;
        } = loader.GetString("SplitViewButtonSave"); 

        /// <summary>
        /// 字幕
        /// </summary>
        public static string SplitViewTabEvent
        {
            get;
        } = loader.GetString("SplitViewTabEvent"); 

        /// <summary>
        /// 设置
        /// </summary>
        public static string SplitViewTabPreferences
        {
            get;
        } = loader.GetString("SplitViewTabPreferences"); 

        /// <summary>
        /// 元数据
        /// </summary>
        public static string SplitViewTabScriptInfo
        {
            get;
        } = loader.GetString("SplitViewTabScriptInfo"); 

        /// <summary>
        /// 样式
        /// </summary>
        public static string SplitViewTabStyle
        {
            get;
        } = loader.GetString("SplitViewTabStyle"); 

        /// <summary>
        /// 无标题
        /// </summary>
        public static string Untitled
        {
            get;
        } = loader.GetString("Untitled"); 
    }
}
