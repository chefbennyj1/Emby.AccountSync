define(["require", "loading", "dialogHelper", "formDialogStyle", "emby-checkbox", "emby-select", "emby-toggle"],
    function (require, loading, dialogHelper) {

        var pluginId = "AFEE16BE-0273-455B-89DA-8AECE378094E";

        function getSyncProfileUsersData(syncProfile) {
            return new Promise((resolve, reject) => {
                var profileCardData = [];
                ApiClient.getJSON(ApiClient.getUrl("Users")).then(
                    (users) => {
                        users.forEach(
                            (user) => {
                                if (user.Id === syncProfile.SyncToAccount) {
                                    profileCardData.push(user);
                                }
                                if (user.Id === syncProfile.SyncFromAccount) {
                                    profileCardData.push(user);
                                }
                            });
                        resolve(profileCardData);
                    }); 
            });
        }

        function getProfileHtml(syncProfileUsersData) {
                
            var html = "";
            html += '<div data-syncTo="' + syncProfileUsersData[0].Id + '" data-syncFrom="' + syncProfileUsersData[1].Id + '" class="syncButtonContainer cardBox visualCardBox syncProfile" style="max-width:322px; width:322px">';
            html += '<div class="cardScalable">';
            html += '<i class="md-icon btnDeleteProfile fab" data-index="0" style="position:absolute; right:2px; margin:1em">close</i>';
            
            html += '<h3 style="margin: 1em;"class=""><i class="md-icon">account_circle</i> From: ' + syncProfileUsersData[0].Name + '</h3>';
          
            html += '<h3 style="margin: 1em;"class=""><i class="md-icon">account_circle</i> To: ' + syncProfileUsersData[1].Name + '</h3>'; 
           
            html += '</div>';
            html += '</div>';

            return html;  
        }

        return function(view) {
            view.addEventListener('viewshow',
                () => {

                    var userOneSelect = view.querySelector('#syncToAccount');
                    var userTwoSelect = view.querySelector('#syncFromAccount');

                    var savedProfileCards = view.querySelector('#syncProfiles');

                    ApiClient.getJSON(ApiClient.getUrl("Users")).then(
                        (users) => {
                            users.forEach(
                                (user) => {
                                   userOneSelect.innerHTML +=
                                        ('<option value="' + user.Id + '" data-name="' + user.Name + '" data-id="' + user.Id + '">' + user.Name + '</option>');
                                   userTwoSelect.innerHTML +=
                                        ('<option value="' + user.Id + '" data-name="' + user.Name + '" data-id="' + user.Id + '">' + user.Name + '</option>');
                                });
                        });

                    ApiClient.getPluginConfiguration(pluginId).then(function (config) {
                        if (config.SyncList) {
                            savedProfileCards.innerHTML = "";
                            config.SyncList.forEach((profile) => {
                                getSyncProfileUsersData(profile).then((result) => {
                                    savedProfileCards.innerHTML += getProfileHtml(result);
                                });
                            });
                        }
                    });


                    view.querySelector('#syncProfiles').addEventListener('click',
                        (e) => {

                            if (e.target.classList.contains('btnDeleteProfile')) {

                                var syncTo   = e.target.closest('div.syncButtonContainer').dataset.syncto;
                                var syncFrom = e.target.closest('div.syncButtonContainer').dataset.syncfrom;

                                var syncList = [];
                               
                                ApiClient.getPluginConfiguration(pluginId).then((config) => {
                                    config.SyncList.forEach((c) => {
                                        if (c.SyncToAccount !== syncTo && c.SyncFromAccount !== syncFrom ) {
                                            syncList.push(c);
                                        }
                                    });

                                    config.SyncList = syncList;

                                    ApiClient.updatePluginConfiguration(pluginId, config).then(
                                        (result) => {
                                            Dashboard.hideLoadingMsg();
                                            Dashboard.processPluginConfigurationUpdateResult(result);
                                        });
                                });

                                e.target.closest('div.syncButtonContainer').remove();
                                return false; 
                            }   

                            if (e.target.closest('div > .syncProfile')) {  

                                var ele = e.target.closest('div > .syncProfile');
                                userOneSelect.value = ele.dataset.syncto;
                                userTwoSelect.value = ele.dataset.syncfrom;
                                   
                            }
                            return false;
                        });


                    view.querySelector('#syncButton').addEventListener('click',
                        (e) => {
                            e.preventDefault;

                            var user1 = userOneSelect.options[userOneSelect.selectedIndex >= 0 ? userOneSelect.selectedIndex : 0].dataset.id;
                            var user2 = userTwoSelect.options[userTwoSelect.selectedIndex >= 0 ? userTwoSelect.selectedIndex : 0].dataset.id;

                            ApiClient.getPluginConfiguration(pluginId).then((config) => {

                                var syncList = [];

                                var syncProfile = {
                                    SyncToAccount: user1,
                                    SyncFromAccount: user2
                                }

                                syncList.push(syncProfile);
                                if (config.SyncList) {
                                    syncList.concat(config.SyncList);
                                }
                                config.SyncList = syncList;

                                ApiClient.updatePluginConfiguration(pluginId, config).then(function (result) {
                                    Dashboard.processPluginConfigurationUpdateResult(result);
                                });

                                savedProfileCards.innerHTML = "";

                                config.SyncList.forEach((profile) => {
                                    getSyncProfileUsersData(profile).then((result) => {
                                        savedProfileCards.innerHTML += getProfileHtml(result);
                                    });
                                });

                            });
                        });  

                });

            view.addEventListener('viewhide',
                () => {

                });

            view.addEventListener('viewdestroy',
                () => {

                });
        }
    });