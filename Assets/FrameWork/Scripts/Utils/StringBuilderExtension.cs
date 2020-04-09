using System.Text;

public static class StringBuilderExtension
{
    public static StringBuilder AppendInt(this StringBuilder sb, int n, int len = 0)
    {
        int l;
        int k;
        if (n == 0)
            l = 0;
        else
            l = (int)System.Math.Floor(System.Math.Log10(n < 0 ? -n : n));
        if (len - 1 > l)
            l = len - 1;
        k = (int)System.Math.Round(System.Math.Pow(10, l));

        do
        {
            if (n < 0)
            {
                sb.Append('-');
                n = -n;
            }
            else
            {
                sb.Append((char)('0' + n / k));
                n %= k;
                k /= 10;
            }
        } while (k > 0);

        return sb;
    }
}
