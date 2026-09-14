SyncVar ve RPC için tipik use-case'ler nelerdir? Hangi durumda hangisini tercih edersiniz?
cevap: 

SyncVar: sunucunun veriyi client'lara paylaştığı ve değiştikçe sürekli senkronize ettiği bir yöntemdir
RPC: network fonksiyonlarını çalıştırmak içindir, gerektiğinde belirli kullanıcıları veya herkesi hedefleyebilir
SyncVar genellikle başka bir oyuncunun canı ve envanter eşyaları gibi verileri eşzamanlı olarak paylaşmak için kullanılırken,
RPC daha çok animasyonları veya lokal görsel efektleri tetiklemek, aksiyonları gerçekleştirmek gibi şeyler için kullanılır.


2. [Command] metodlarını tasarlarken nelere dikkat edersiniz?
cevap :
Clientlar sadece satın alınacak belirli bir eşyanın IDsini iletmek gibi istekler göndermelidir.
sunucu ise gerçek değerleri kendi güvenli veri tabanından bulmak için bu IDyi kullanmalı ve eylemi gerçekleştirmeden önce (oyuncunun yeterli parası var mı?)
gibi tüm oyun kurallarını bağımsız olarak doğrulamalıdır.

[Command]
void CmdBuyItem(int itemId, int price)
{
    inventory.Add(itemId);
    gold -= price;
}
Bu kodda güvenlik, authority veya oyun mantığı açısından herhangi bir problem görüyor musunuz? Varsa açıklayınız
cevap :
Client price değerini parametre olarak kendisi gönderiyor.bir oyuncu fiyatı 0 veya -1000 gibi negatif bir değer göndererek eşyaları bedavaya alabilir, veya üstüne altın bile kazanabilir.

Sunucu, oyuncunun o eşyayı almaya yetecek kadar altını olup olmadığını kontrol etmiyor (if (gold >= price) eksik). Oyuncunun hiç parası olmasa bile eşyayı alabilir ve bakiyesi eksiye düşer.
