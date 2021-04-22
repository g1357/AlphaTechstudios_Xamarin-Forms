using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Models
{
    /// <summary>
    /// Модель данных диалога
    /// </summary>
    public class ConversationModel : BaseModel
    {
        /// <summary>
        /// Ответы в диалога.
        /// </summary>
        public ICollection<ConversationReplyModel> ConversationsReplies { get; set; }

        /// <summary>
        /// Идентификатор первого пользователя.
        /// </summary>
        public long UserOneID { get; set; }

        /// <summary>
        /// Идентификатор второго пользователя.
        /// </summary>
        public long UserTwoID { get; set; }
    }
}
