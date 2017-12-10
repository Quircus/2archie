using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.LuisBot
{
	// For more information about this template visit http://aka.ms/azurebots-csharp-luis

	// I remove the following when I'm pushing to Azure, which applies Luis keys privately through a config manager.
	// I need to use the keys directly when I'm testing locally on localhost:3984 but still want to access my trained LUIS services.
	//[LuisModel("e5ffc9ee-42af-4dad-9833-1ad6ded1ba09", "4a9bf25a264448128c3d033bc46b0b3b")]

	[Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
		public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
		{
		}

		[LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
            context.Wait(MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "MyIntent" with the name of your newly created intent in the following handler
        [LuisIntent("MyIntent")]
        public async Task MyIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached the MyIntent intent. You said: {result.Query}"); //
            context.Wait(MessageReceived);
        }

		// Go to https://luis.ai and create a new intent, then train/publish your luis app.
		// Finally replace "MyIntent" with the name of your newly created intent in the following handler
		[LuisIntent("ReserveRoom")]
		public async Task ReserveRoomIntent(IDialogContext context, LuisResult result)
		{
			await context.PostAsync($"You have reached the ReserveRoom intent. You said: {result.Query}"); //
			context.Wait(MessageReceived);
		}

		// Go to https://luis.ai and create a new intent, then train/publish your luis app.
		// Finally replace "MyIntent" with the name of your newly created intent in the following handler
		[LuisIntent("SearchRooms")]
		public async Task SearchRoomsIntent(IDialogContext context, LuisResult result)
		{
			string message = "";
			if (result.Entities.Count != 0)
			{
				message = "Okay, let's see what we have with a " + result.Entities[0].Entity;
			}
			else
			{
				message = "You wanted to search for something, yes? Do you want a room with single, double, or twin beds?";
			}


			await context.PostAsync(message);
			context.Wait(MessageReceived);

		}

		// Go to https://luis.ai and create a new intent, then train/publish your luis app.
		// Finally replace "MyIntent" with the name of your newly created intent in the following handler
		[LuisIntent("Help")]
		public async Task HelpIntent(IDialogContext context, LuisResult result)
		{
			await context.PostAsync($"You have reached the Help intent. You said: {result.Query}"); //
			context.Wait(MessageReceived);
		}
	}
}