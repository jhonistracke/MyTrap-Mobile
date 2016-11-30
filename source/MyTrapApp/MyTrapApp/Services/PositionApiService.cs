using MyTrapApp.Models.Base;
using MyTrapApp.Models.Request;
using MyTrapApp.Repository;
using MyTrapApp.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTrapApp.Services
{
    public class PositionApiService
    {
        public static void Save(PositionApiRequest request)
        {
            PositionRepository.Save(request);
        }

        public static List<PositionApiRequest> GetPositionsToSend()
        {
            return PositionRepository.GetAll();
        }

        public static void Remove(PositionApiRequest request)
        {
            PositionRepository.Delete(request.Id);
        }

        public static async Task<bool> Send(PositionApiRequest request, bool checkStored)
        {
            bool result = false;

            try
            {
                var response = await HttpController.PostData<BaseApiResult>(ApiMethods.SendPositionUrl(), request);

                if (ResponseValidator.Validate(response))
                {
                    result = true;

                    if (checkStored)
                    {
                        List<PositionApiRequest> storedPositions = GetPositionsToSend();

                        if (storedPositions != null && storedPositions.Count > 0)
                        {
                            foreach (PositionApiRequest posToSend in storedPositions)
                            {
                                bool isStoredSend = await Send(posToSend, false);

                                if (isStoredSend)
                                {
                                    Remove(posToSend);
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
    }
}