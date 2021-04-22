using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Модели данных приложения
/// </summary>
namespace ChatApp.Models
{
	/// <summary>
	/// Модель данных ответа в диалоге
	/// </summary>
	public class ConversationReplyModel : BaseModel
	{
		/// <summary>
		/// Идентификатор пользователя отправителя
		/// </summary>
		public long SenderUserId { get; set; }

		/// <summary>
		/// Содержание ответа в диалогер
		/// </summary>
		public string Content { get; set; }

		public long ConversationID { get; set; }

		/// <summary>
		/// Данные о диалоге 
		/// </summary>
		//[ForeignKey("ConversationId")]
		public ConversationModel Conversation { get; set; }


	}
}
