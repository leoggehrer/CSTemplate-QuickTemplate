//@BaseCode
//MdStart

namespace QuickTemplate.AspMvc.Models.View
{
    public partial class SubmitCmd
    {
        public SubmitCmd()
        {
            Constructing();
            Constructed();
        }
        partial void Constructing();
        partial void Constructed();

        public bool RightAlign { get; set; } = false;
        public string SubmitText { get; set; } = "Save";
        public string SubmitCss { get; set; } = "btn btn-primary";
        public string SubmitStyle { get; set; } = "min-width: 8em;";
        public string SubmitAction { get; set; } = string.Empty;
    }
}
//MdEnd
