using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.Data;
using System.Linq;

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

		private static IList<Attachment> GetCardsAttachments(IQueryable cards)
		{
			IList<Attachment> selections = new List<Attachment> { };

			foreach (Room c in cards)
			{
				selections.Add(GetHeroCard(c.Name, c.BedType + ". Currently available for $" + c.Rates + " per night", c.Details, new CardImage(c.RoomUrl), new CardAction(ActionTypes.OpenUrl, "Reserve Now", value: "http://2archie.azurewebsites.net/")));
			}

			return selections;
		}


		private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
		{
			var heroCard = new HeroCard
			{
				Title = title,
				Subtitle = subtitle,
				Text = text,
				Images = new List<CardImage>() { cardImage },
				Buttons = new List<CardAction>() { cardAction },
			};

			return heroCard.ToAttachment();
		}

		[LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I'm afraid I don't understand that. You said: {result.Query}"); //
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
				string searchString = result.Entities[0].Entity;
				message = "Okay, let's see what we have with a " + searchString;
				await context.PostAsync(message);

				// query db for rooms with entity search and order by price


				using (RoomDataContext db = new RoomDataContext())
				{
					IQueryable<Room> rooms = db.Rooms;

					if (!String.IsNullOrEmpty(searchString))
					{
						rooms = rooms.Where(s => s.BedType.Contains(searchString)).OrderByDescending(t => t.Rates);
					}



					var reply = context.MakeMessage();

					reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
					reply.Attachments = GetCardsAttachments(rooms);

					await context.PostAsync(reply);
				}

			}
			else
			{
				message = "You wanted to search for something, yes? Ask me to search for a room, specifying a single, double, or twin bed.";
				await context.PostAsync(message);
			}
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