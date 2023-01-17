//Copyright(c) XFP Group and Contributors. All rights reserved.
//Licensed under the MIT License.   

namespace XFP.Impact_Ultimate.Model
{
    public class MenuInfo
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _GroupId;

        public string GroupId
        {
            get { return _GroupId; }
            set { _GroupId = value; }
        }

        private string _GroupName;

        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; }
        }
    }
}
