
namespace QuickSockets.Exceptions;

internal class WrongResponseException : Exception
{
    public WrongResponseException(string wrongResponse)
    {
        _wrongResponse = wrongResponse;
    }

    private string _wrongResponse;
    public string WrongResponse => _wrongResponse;
}
