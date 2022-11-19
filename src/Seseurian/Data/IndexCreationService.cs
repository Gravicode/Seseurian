

using Seseurian.Data;
using Seseurian.Models;

namespace Redis.OM.Skeleton.HostedServices;

public class IndexCreationService 
{
    private readonly RedisConnectionProvider _provider;
    public IndexCreationService()
    {
        _provider = new RedisConnectionProvider(AppConstants.RedisCon);

    }

    public async Task CreateIndex()
    {
        await _provider.Connection.CreateIndexAsync(typeof(Log));
        await _provider.Connection.CreateIndexAsync(typeof(Post));
        await _provider.Connection.CreateIndexAsync(typeof(Trending));
        await _provider.Connection.CreateIndexAsync(typeof(UserProfile));
        await _provider.Connection.CreateIndexAsync(typeof(MessageBox));
        await _provider.Connection.CreateIndexAsync(typeof(Notification));
    }

}