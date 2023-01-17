//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.Impact_Ultimate.Utlis.Base
{
    public enum ModuleVersion
    { 
        /// <summary>
        /// 无版本
        /// </summary>
        None = 0,

        /// <summary>
        /// 基础版本(Akebi Plus时置基础版本)
        /// </summary>
        BasicEdition = 1,

        /// <summary>
        /// Beta版本(Impact_Ultimate时置Beta版本)
        /// </summary>
        BetaEdition = 2,

        /// <summary>
        /// 开发者版本(申请开发者版本 将添加进 [代码管理功能])
        /// </summary>
        DevelopmentEdition = 3,
    }
}
