mergeInto(LibraryManager.library, {

  HideLoadingPage: function() {
    hideLoadingPage();
  },

  showRewardedAd: function(){
    triggerRewarded();
  },

  triggerMilestone: function(type, data){
    try {
      var milestone = {};
      milestone.type = UTF8ToString(type);
      milestone.meta = JSON.parse(UTF8ToString(data));
      milestone.level = 1;
      milestone.score = 0;
      milestone.highScore = 0;
      console.log("triggerMilestone: " + milestone);
      gameAnalytics("game_milestone", JSON.stringify(milestone));
    } catch(err){
      console.log("Error in triggerMilestone: " + err);
    }
  },

  triggerTransactions: function(type, data){
    try {
      var transaction = {};
      transaction.type = UTF8ToString(type);
      transaction.meta = JSON.parse(UTF8ToString(data));
      transaction.level = 1;
      transaction.score = 0;
      transaction.highScore = 0;
      console.log("triggerMilestone: " + transaction);
      gameAnalytics("ingame_transactions", JSON.stringify(transaction));
    } catch(err){
      console.log("Error in triggerTransactions: " + err);
    }
  },
  
  triggerAnalytics: function(action, data){
    try {
      var jsonStr = UTF8ToString(data);
      gameAnalytics(action, data);
    } catch(err){
      console.log("Error in triggerAnalytics: " + err);
    }
  },
});