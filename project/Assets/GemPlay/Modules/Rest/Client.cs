using UnityEngine;

namespace GemPlay.Modules.Rest
{
    public static class Client
    {
        public static RSG.IPromise<Proyecto26.ResponseHelper> Request(Proyecto26.RequestHelper request)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            Debug.Log("Request: " + request.Method + " " + request.Uri);
            
            if (request.BodyString != null)
            {
                Debug.Log("Request body: " + request.BodyString);
            }

            if (request.Timeout != null)
            {
                Debug.Log("Request will time out after " + request.Timeout + " seconds.");
            }
            
            var responsePromise = new RSG.Promise<Proyecto26.ResponseHelper>();

            Proyecto26.RestClient.Request(request)
                .Then(response => 
                {
                    Debug.Log("Response received after " + stopwatch.Elapsed.Milliseconds + " milliseconds.");

                    Debug.Log("Response status code: " + response.StatusCode);

                    if (response.Text != null)
                    {
                        Debug.Log("Response text: " + response.Text);
                    }

                    responsePromise.Resolve(response);
                })
                .Catch(error =>
                {
                    Debug.LogError(error.Message + "\n" + error.StackTrace);

                    throw error;
                });

            return responsePromise;
        }
    }
}
