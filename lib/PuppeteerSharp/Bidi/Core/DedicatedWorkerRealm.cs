// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Collections.Concurrent;
using System.Linq;
using PuppeteerSharp.Helpers;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi.Core;

internal class DedicatedWorkerRealm : Realm, IDedicatedWorkerOwnerRealm
{
    private readonly ConcurrentSet<IDedicatedWorkerOwnerRealm> _owners = [];
    private readonly ConcurrentDictionary<string, DedicatedWorkerRealm> _workers = [];

    private DedicatedWorkerRealm(BrowsingContext context, IDedicatedWorkerOwnerRealm owner, string id, string origin)
    : base(context, id, origin)
    {
        _owners.Add(owner);
    }

    public event EventHandler<WorkerRealmEventArgs> Worker;

    public override Session Session => _owners.FirstOrDefault()?.Session;

    public static DedicatedWorkerRealm From(BrowsingContext context, IDedicatedWorkerOwnerRealm owner, string id, string origin)
    {
        var realm = new DedicatedWorkerRealm(context, owner, id, origin);
        realm.Initialize();
        return realm;
    }

    public override void Dispose()
    {
        foreach (var worker in _workers.Values)
        {
            worker.Dispose();
        }

        base.Dispose();
    }

    private void Initialize()
    {
        // Listen to realm destruction
        Session.Driver.Script.OnRealmDestroyed.AddObserver(OnRealmDestroyed);

        // Listen to nested worker creation
        Session.Driver.Script.OnRealmCreated.AddObserver(OnDedicatedRealmCreated);
    }

    private void OnRealmDestroyed(RealmDestroyedEventArgs args)
    {
        if (args.RealmId != Id)
        {
            return;
        }

        Dispose("Realm already destroyed.");
    }

    private void OnDedicatedRealmCreated(RealmCreatedEventArgs args)
    {
        if (args.Type != RealmType.DedicatedWorker)
        {
            return;
        }

        var dedicatedWorkerInfo = args.As<DedicatedWorkerRealmInfo>();

        if (!dedicatedWorkerInfo.Owners.Contains(Id))
        {
            return;
        }

        var realm = DedicatedWorkerRealm.From(Context, this, dedicatedWorkerInfo.RealmId, dedicatedWorkerInfo.Origin);
        _workers.TryAdd(realm.Id, realm);
        realm.Destroyed += (sender, args) => _workers.TryRemove(realm.Id, out _);
        OnWorker(realm);
    }

    private void OnWorker(DedicatedWorkerRealm realm) => Worker?.Invoke(this, new WorkerRealmEventArgs(realm));
}

#endif
