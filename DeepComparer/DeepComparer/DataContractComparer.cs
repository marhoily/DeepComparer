namespace DeepComparer
{
    public sealed class DataContractComparer
    {
        public bool Compare(object x, object y)
        {
            foreach (var p in x.GetType().GetProperties())
            {
                var xV = p.GetValue(x, null);
                var yV = p.GetValue(y, null);
                if (!Equals(xV, yV)) return false;
            }
            return true;
        }
    }
}