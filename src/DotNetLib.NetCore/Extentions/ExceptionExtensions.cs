namespace DotNetLib.NetCore.Extentions
{
    public static partial class ExceptionExtensions
    {
        public static string GetMessageAsString(this Exception ex)
        {
            var errorMessage = string.Join("; ", ex.GetMessageList());
            return errorMessage;
        }

        public static IEnumerable<string> GetMessageList(this Exception ex)
        {
            if (ex == null)
            {
                yield break;
            }

            yield return ex.Message;


            IEnumerable<Exception> innerExceptions = System.Linq.Enumerable.Empty<Exception>();

            if (ex is AggregateException && (ex as AggregateException).InnerExceptions.Any())
            {
                innerExceptions = (ex as AggregateException).InnerExceptions;
            }
            else if (ex.InnerException != null)
            {
                innerExceptions = new Exception[] { ex.InnerException };
            }

            foreach (var innerEx in innerExceptions)
            {
                foreach (string msg in innerEx.GetMessageList())
                {
                    yield return msg;
                }
            }
        }

    }
}
