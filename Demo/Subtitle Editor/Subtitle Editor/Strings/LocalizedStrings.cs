using Windows.ApplicationModel.Resources;

namespace SubtitleEditor
{    
    public static class LocalizedStrings
    {
        private static readonly ResourceLoader loader = new ResourceLoader();

        public static string GetString(string resourceKey) => loader.GetString(resourceKey);

        /// <summary>
        /// 修改{0}
        /// </summary>
        public static string ActionSetScriptInfo
        {
            get;
        } = loader.GetString("ActionSetScriptInfo"); 

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
        /// 移动后续字幕。
        /// </summary>
        public static string CollisionStyleNormal
        {
            get;
        } = loader.GetString("CollisionStyleNormal"); 

        /// <summary>
        /// 为后续字幕预留空间。
        /// </summary>
        public static string CollisionStyleReverse
        {
            get;
        } = loader.GetString("CollisionStyleReverse"); 

        /// <summary>
        /// 撤销 {0}
        /// </summary>
        public static string DocumentRedo
        {
            get;
        } = loader.GetString("DocumentRedo"); 

        /// <summary>
        /// 重做 {0}
        /// </summary>
        public static string DocumentUndo
        {
            get;
        } = loader.GetString("DocumentUndo"); 

        /// <summary>
        /// 深色
        /// </summary>
        public static string ElementThemeDark
        {
            get;
        } = loader.GetString("ElementThemeDark"); 

        /// <summary>
        /// 浅色
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
        /// 冲突处理
        /// </summary>
        public static string ScriptInfoCollisions
        {
            get;
        } = loader.GetString("ScriptInfoCollisions"); 

        /// <summary>
        /// 编辑
        /// </summary>
        public static string ScriptInfoOriginalEditing
        {
            get;
        } = loader.GetString("ScriptInfoOriginalEditing"); 

        /// <summary>
        /// 脚本
        /// </summary>
        public static string ScriptInfoOriginalScript
        {
            get;
        } = loader.GetString("ScriptInfoOriginalScript"); 

        /// <summary>
        /// 时间轴
        /// </summary>
        public static string ScriptInfoOriginalTiming
        {
            get;
        } = loader.GetString("ScriptInfoOriginalTiming"); 

        /// <summary>
        /// 翻译
        /// </summary>
        public static string ScriptInfoOriginalTranslation
        {
            get;
        } = loader.GetString("ScriptInfoOriginalTranslation"); 

        /// <summary>
        /// 色彩深度
        /// </summary>
        public static string ScriptInfoPlayDepth
        {
            get;
        } = loader.GetString("ScriptInfoPlayDepth"); 

        /// <summary>
        /// 水平分辨率
        /// </summary>
        public static string ScriptInfoPlayResX
        {
            get;
        } = loader.GetString("ScriptInfoPlayResX"); 

        /// <summary>
        /// 垂直分辨率
        /// </summary>
        public static string ScriptInfoPlayResY
        {
            get;
        } = loader.GetString("ScriptInfoPlayResY"); 

        /// <summary>
        /// 缩放边框和阴影
        /// </summary>
        public static string ScriptInfoScaledBorderAndShadow
        {
            get;
        } = loader.GetString("ScriptInfoScaledBorderAndShadow"); 

        /// <summary>
        /// 修订
        /// </summary>
        public static string ScriptInfoScriptUpdatedBy
        {
            get;
        } = loader.GetString("ScriptInfoScriptUpdatedBy"); 

        /// <summary>
        /// 同步点
        /// </summary>
        public static string ScriptInfoSynchPoint
        {
            get;
        } = loader.GetString("ScriptInfoSynchPoint"); 

        /// <summary>
        /// 播放速度
        /// </summary>
        public static string ScriptInfoTimer
        {
            get;
        } = loader.GetString("ScriptInfoTimer"); 

        /// <summary>
        /// 标题
        /// </summary>
        public static string ScriptInfoTitle
        {
            get;
        } = loader.GetString("ScriptInfoTitle"); 

        /// <summary>
        /// 修订信息
        /// </summary>
        public static string ScriptInfoUpdateDetails
        {
            get;
        } = loader.GetString("ScriptInfoUpdateDetails"); 

        /// <summary>
        /// 换行方式
        /// </summary>
        public static string ScriptInfoWrapStyle
        {
            get;
        } = loader.GetString("ScriptInfoWrapStyle"); 

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
        /// 脚本信息
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

        /// <summary>
        /// 自上而下填充各行，“\N”换行。
        /// </summary>
        public static string WrapStyleEndOfLine
        {
            get;
        } = loader.GetString("WrapStyleEndOfLine"); 

        /// <summary>
        /// 不自动折行，“\N”和“\n”换行。
        /// </summary>
        public static string WrapStyleNone
        {
            get;
        } = loader.GetString("WrapStyleNone"); 

        /// <summary>
        /// 平均分布各行，上行较宽，“\N”换行。
        /// </summary>
        public static string WrapStyleSmart
        {
            get;
        } = loader.GetString("WrapStyleSmart"); 

        /// <summary>
        /// 平均分布各行，下行较宽，“\N”换行。
        /// </summary>
        public static string WrapStyleSmart2
        {
            get;
        } = loader.GetString("WrapStyleSmart2"); 
    }
}
