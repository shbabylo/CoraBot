﻿using Greyshirt.Dialogs.Preferences;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.Prompts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greyshirt.Dialogs.NewUser
{
    public class NewUserDialog : DialogBase
    {
        public static string Name = typeof(NewUserDialog).FullName;

        public NewUserDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration)
            : base(state, dialogs, api, configuration) { }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        // Welcome and ask for consent.
                        var choices = new List<Choice>();
                        Shared.Phrases.NewUser.ConsentOptions.ForEach(s => choices.Add(new Choice { Value = s }));

                        return await dialogContext.PromptAsync(
                            Prompt.ChoicePrompt,
                            new PromptOptions()
                            {
                                Prompt = Shared.Phrases.Greeting.WelcomeNew,
                                Choices = choices
                            },
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        var result = (dialogContext.Result as FoundChoice).Value;
                        var greyshirt = await api.GetGreyshirt(dialogContext.Context);

                        if (string.Equals(result, Shared.Phrases.NewUser.ConsentNo, StringComparison.OrdinalIgnoreCase))
                        {
                            // Did not consent. Delete their user record.
                            await this.api.Delete(greyshirt);

                            await Messages.SendAsync(Shared.Phrases.NewUser.NoConsent, dialogContext.Context, cancellationToken);
                            return await dialogContext.EndDialogAsync(false, cancellationToken);
                        }

                        greyshirt.IsConsentGiven = true;
                        await this.api.Update(greyshirt);

                        await Messages.SendAsync(Shared.Phrases.NewUser.Consent, dialogContext.Context, cancellationToken);
                        return await BeginDialogAsync(dialogContext, GreyshirtRegisterDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        return await BeginDialogAsync(dialogContext, LocationDialog.Name, null, cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        await Messages.SendAsync(Phrases.NewUser.RegistrationComplete, turnContext, cancellationToken);
                        return await dialogContext.EndDialogAsync(true, cancellationToken);
                    },
                });
            });
        }
    }
}
