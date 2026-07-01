namespace Education.Domain.Practicals;

public static class GradingPolicy
{
    public static int GetGrade(double? score, double? maxScore, double five, double four, double three)
    {
        if (score is null || maxScore is null || maxScore == 0)
        {
            return 0;
        }

        var percent = score / maxScore;
        if (percent >= five / 100)
        {
            return 5;
        }

        if (percent >= four / 100)
        {
            return 4;
        }

        if (percent >= three / 100)
        {
            return 3;
        }

        return 2;
    }
}
