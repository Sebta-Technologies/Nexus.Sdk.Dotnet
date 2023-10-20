﻿using Nexus.SDK.Shared.Responses;

namespace Nexus.SDK.Shared.Facades;

public class MailsFacade : ServerFacade, IMailsFacade
{
    public MailsFacade(IServerProvider provider) : base(provider)
    {
    }

    /// <summary>
    /// List Mails based on query paramaters
    /// </summary>
    /// <param name="query">Query parameters to filter on. Check the Nexus API documentation for possible filtering parameters.</param>
    /// <returns>
    /// Paged list of Partner's mails
    /// </returns>
    public async Task<PagedResponse<MailsResponse>> Get(IDictionary<string, string>? query)
    {
        return await _provider.GetMails(query);
    }
}
