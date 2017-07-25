using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Office.Interop.Outlook;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Bot_Application1.Dialogs
{
	[Serializable]
	public class RootDialog : IDialog<object>
	{
		public Task StartAsync(IDialogContext context)
		{
			context.Wait(MessageReceivedAsync);

			return Task.CompletedTask;
		}

		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
		{
			var activity = await result as Activity;
			string body = "welcome";
			// calculate something for us to return
			int length = (activity.Text ?? string.Empty).Length;
			
			// return our reply to the user
			

			//((Microsoft.Bot.Connector.Activity)context.Activity). = "list";
			await context.PostAsync(body);

			context.Wait(MessageReceivedAsync);
		}

		public DataTable PullData()
		{
			string connString = "Server=tcp:akashfirstserver.database.windows.net,1433;Initial Catalog=QTtracker; User ID=akashkh;Password=PoloMohit@12345;";
			string query = "select * from SLAdetails";

			SqlConnection conn = new SqlConnection(connString);
			SqlCommand cmd = new SqlCommand(query, conn);
			conn.Open();
			DataTable dt = new DataTable();
			// create data adapter
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			// this will query your database and return the result to your datatable
			da.Fill(dt);
			conn.Close();
			da.Dispose();

			return dt;
		}
	}

	[Serializable]
	public class CardsDialog : IDialog<object>
	{
		private const string HeroCard = "Hero card";
		private const string ThumbnailCard = "Thumbnail card";
		private const string ReceiptCard = "Receipt card";
		private const string SigninCard = "Sign-in card";
		private const string AnimationCard = "Animation card";
		private const string VideoCard = "Video card";
		private const string AudioCard = "Audio card";

		private IEnumerable<string> options = new List<string> ();

		public async Task StartAsync(IDialogContext context)
		{
			context.Wait(this.MessageReceivedAsync);
		}

		public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
		{
			var message = await result;

				
				Activity replyToConversation = GetHeroCard(context, "Please help");
			await context.PostAsync(replyToConversation);
			context.Wait(MessageReceivedAsync);


		}

		//public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
		//{
		//	var selectedCard = await result;

		//	var message = context.MakeMessage();

		//	var attachment = GetHeroCard();
		//	message.Attachments.Add(attachment);

		//	await context.PostAsync(message);

		//	context.Wait(this.MessageReceivedAsync);
		//}

		//private static Microsoft.Bot.Connector.Attachment GetSelectedCard(string selectedCard)
		//{
		//	switch (selectedCard)
		//	{
		//		case HeroCard:
		//			return GetHeroCard();
		//		case ThumbnailCard:
		//			return GetThumbnailCard();
		//		case ReceiptCard:
		//			return GetReceiptCard();
		//		case SigninCard:
		//			return GetSigninCard();
		//		case AnimationCard:
		//			return GetAnimationCard();
		//		case VideoCard:
		//			return GetVideoCard();
		//		case AudioCard:
		//			return GetAudioCard();

		//		default:
		//			return GetHeroCard();
		//	}
		//}

		private static Activity GetHeroCard(IDialogContext context, string strText)
		{
			Activity replyToConversation = (Activity)context.MakeMessage();
			replyToConversation.Text = strText;
			replyToConversation.Recipient = replyToConversation.Recipient;
			replyToConversation.Type = "message";
			DataTable dt = PullData();
			string[] Sr = dt.AsEnumerable().Select(r => r.Field<string>("SRNumber")).ToArray();
			int Srcount = Sr.Count();


			List<CardAction> cardbuttons;
			
				cardbuttons = CreateButtons(0, Srcount);
				var heroCard = new HeroCard
				{
					Title = "Cases Need Attention",
					Subtitle = "Near IR",
					//Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
					//Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
					Buttons = cardbuttons
					//Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Get Started", value: "https://docs.microsoft.com/bot-framework") }
				};

			Microsoft.Bot.Connector.Attachment plAttachment = heroCard.ToAttachment();
			replyToConversation.Attachments.Add(plAttachment);
			replyToConversation.AttachmentLayout = "list";
			// Return the reply to the calling method
			return replyToConversation;


		}


		private static List<CardAction> CreateButtons(int start, int end)
		{
			DataTable dt = PullData();
			string[] Sr = dt.AsEnumerable().Select(r => r.Field<string>("SRNumber")).ToArray();
			DateTime[] Sr1 = dt.AsEnumerable().Select(r => r.Field<DateTime>("SLADateTime")).ToArray();
			
			// Create 5 CardAction buttons 
			// and return to the calling method 
			List<CardAction> cardButtons = new List<CardAction>();
			for (int i = start; i < end; i++)
			{
				string CurrentNumber = Convert.ToString(i);
				CardAction CardButton = new CardAction()
				{
					Type = "imBack",
					Title = Sr[i].ToString() + " IR @" + Sr1[i].ToString(),
					Value = Sr[i].ToString() 
				};
				cardButtons.Add(CardButton);
			}
			return cardButtons;
		}
	

		public static DataTable PullData()
		{
			string connString = "Server=tcp:akashfirstserver.database.windows.net,1433;Initial Catalog=QTtracker; User ID=akashkh;Password=PoloMohit@12345;";
			string query = "Select SRNumber, SLADateTime from SLAdetails where sladatetime > (Select CAST(SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30') AS DATETIME) ) and isActive = 1 order by sladatetime";

			SqlConnection conn = new SqlConnection(connString);
			SqlCommand cmd = new SqlCommand(query, conn);
			conn.Open();
			DataTable dt = new DataTable();
			// create data adapter
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			// this will query your database and return the result to your datatable
			da.Fill(dt);
			conn.Close();
			da.Dispose();

			return dt;
		}
	}
}