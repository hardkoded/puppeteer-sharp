namespace PuppeteerSharp
{
    public class CoverageEntryRange
    {
        public int Start { get; internal set; }
        public int End { get; internal set; }

        public override bool Equals(object obj)
        {
            if (obj == null && GetType() != obj.GetType())
            {
                return false;
            }

            var range = obj as CoverageEntryRange;

            return range.Start == Start &&
               range.End == End;
        }

        public override int GetHashCode() => Start.GetHashCode() * 397 ^ End.GetHashCode() * 397;
    }
}