using System.Text;

namespace Extension.Utils
{
    class RichtextBuilder
    {
        readonly StringBuilder Text;

        public RichtextBuilder()
        {
            Text = new StringBuilder();
        }

        public RichtextBuilder Append(char c)
        {
            Text.Append(c);
            return this;
        }

        public RichtextBuilder Append(string s)
        {
            Text.Append(s);
            return this;
        }

        public RichtextBuilder AppendLine()
        {
            Text.Append("\n");
            return this;
        }

        public RichtextBuilder AppendLine(string s)
        {
            Text.Append(s);
            Text.Append("\n");
            return this;
        }

        public RichtextBuilder AppendImg(string imgName)
        {
            Text.Append("<img src=\"");
            Text.Append(imgName);
            Text.Append("\">");
            return this;
        }

        public RichtextBuilder AppendBulletpointL1(string s)
        {
            Text.Append("  - ");
            Text.Append(s);
            Text.Append("\n");
            return this;
        }

        public RichtextBuilder AppendBulletpointL2(string s)
        {
            Text.Append("    - ");
            Text.Append(s);
            Text.Append("\n");
            return this;
        }

        public RichtextBuilder StartStyle(string styleName)
        {
            Text.Append("<span style=\"");
            Text.Append(styleName);
            Text.Append("\">");
            return this;
        }

        public RichtextBuilder EndStyle()
        {
            Text.Append("</span>");
            return this;
        }

        public RichtextBuilder AppendDoubleLine()
        {
            Text.Append("\n \n");
            return this;
        }

        public override string ToString()
        {
            return Text.ToString();
        }
    }
}
