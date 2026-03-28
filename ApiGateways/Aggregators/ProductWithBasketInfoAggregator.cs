using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class ProductWithBasketInfoAggregator : IDefinedAggregator
{
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {
        var productResponse = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
        var basketResponse = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

        // Here you can combine the responses in the way you want.
        // For simplicity, let's just concatenate them.
        var aggregateContent = $"{productResponse}\n{basketResponse}";

        return new DownstreamResponse(new StringContent(aggregateContent), HttpStatusCode.OK, new List<Header>(), "OK");
    }
}
