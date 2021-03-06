﻿using Bot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Models;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
    public class FeedbackDialog : DialogBase
    {
        public static string Name = typeof(FeedbackDialog).FullName;

        public FeedbackDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration)
        {
        }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Prompt for feedback.
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions { Prompt = Phrases.Feedback.GetFeedback },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var user = await api.GetUser(dialogContext.Context);

                        var feedback = new Feedback { CreatedById = user.Id,
                        Text = (string)dialogContext.Result };
                        await api.Create(feedback);

                        await Messages.SendAsync(Phrases.Feedback.Thanks, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(null, cancellationToken);
                    }
                });
            });
        }
    }
}
