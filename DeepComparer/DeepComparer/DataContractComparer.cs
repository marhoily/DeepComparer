namespace DeepComparer
{
    public sealed class DataContractComparer
    {
        public bool Compare(object x, object y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            var type = x.GetType();
            if (y.GetType() != type)
                return false;
            foreach (var p in type.GetProperties())
            {
                var xV = p.GetValue(x, null);
                var yV = p.GetValue(y, null);
                if (!Equals(xV, yV)) return false;
            }
            return true;
        }
    }
}