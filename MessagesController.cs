///receives the message from the user (Microsoft Bot Framework for CSharp)
///receives the message from the user

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs; //access the relevant classes
using Microsoft.Bot.Connector;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        //public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        //{
        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());      //here we redirect to the RootDialog.cs
        //    }
        //    else
        //    {
        //        HandleSystemMessage(activity);
        //    }
        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    return response;
        //}

        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // Check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new EchoDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

    }

    //EchoDIalog replies the user with what they said and in addition it also track the dialog state with a simple number of replies:
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        //this function is called when a new message is received, it also evaluates the user's message before responding:
        //If the user's message is "reset", the built-in PromptDialog.Confirm prompt spawns a sub-dialog that asks the user to confirm the count reset.
        //The sub-dialog has its own private state that does not interfere with the parent dialog's state. When the user responds to the prompt, the result of the sub-dialog is passed to the AfterResetAsync
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument) 
        {
            var message = await argument;
            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.None);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument) //The IDialogContext interface that is passed into each dialog method provides access to the services that a dialog requires to save state and communicate with the channel. 
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }



}
