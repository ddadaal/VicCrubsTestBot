using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace VicCrubsTestBot.Dialogs
{
    [LuisModel("6efb746f-8773-4725-908c-7881abd27c12", "ccad6263e2cd434bab371a2562823097")]
    [Serializable]
    public class TestDialog : LuisDialog<object>
    {
        public TestDialog() { }
        public TestDialog(ILuisService service) : base(service) { }
        
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"貌似我不明白这句话……" + string.Join("，", result.Intents.Select(x => x.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        public bool TryToFindLocation(LuisResult result, out String location)
        {
            location = "";
            EntityRecommendation title;
            if (result.TryFindEntity("地点", out title))
            {
                location = title.Entity;
            }
            else
            {
                location = "";
            }
            return !location.Equals("");
        }
        [LuisIntent("查询天气")]
        public async Task QueryWeather(IDialogContext context, LuisResult result)
        {
            string location = "";
            string replyString = "";
            if (TryToFindLocation(result, out location))
            {
                replyString =  GetWeather(location);
                await context.PostAsync(replyString);
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("亲你要查询哪个地方的天气信息呢，快把城市的名字发给我吧");
                context.Wait(WaitForCityName);
            }
        }

        public async Task WaitForCityName(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var messages = await item;
            string city = messages.Text;
            await context.PostAsync(GetWeather(city));
            context.Wait(MessageReceived);
        }
        public string GetWeather(string location)
        {
            return $"你要查{location}的天气！";
        }
    }
}