document.addEventListener('DOMContentLoaded', () => {
  const cartCountElement = document.getElementById('cart-count');
  const cartItemsElement = document.getElementById('cart-items');
  const cartTotalPriceElement = document.getElementById('cart-total-price');
  let cart = [];

  function updateCart() {
    cartCountElement.textContent = cart.reduce((sum, item) => sum + item.quantity, 0);
    cartItemsElement.innerHTML = '';
    let total = 0;
    cart.forEach(item => {
      const div = document.createElement('div');
      div.className = 'cart-row';
      div.innerHTML = `
        <span class="cart-column">${item.name}</span>
        <span class="cart-column">$${item.price.toFixed(2)}</span>
        <span class="cart-column">${item.quantity}</span>
        <span class="cart-column">$${(item.price * item.quantity).toFixed(2)}</span>
      `;
      cartItemsElement.appendChild(div);
      total += item.price * item.quantity;
    });
    cartTotalPriceElement.textContent = `$${total.toFixed(2)}`;
  }

  document.querySelectorAll('.producto button').forEach(button => {
    button.addEventListener('click', () => {
      const producto = button.closest('.producto');
      const name = producto.querySelector('h2').textContent;
      const price = parseFloat(producto.querySelector('.precio').textContent.replace('$', ''));
      const existingItem = cart.find(item => item.name === name);
      if (existingItem) {
        existingItem.quantity += 1;
      } else {
        cart.push({ name, price, quantity: 1 });
      }
      updateCart();
    });
  });

  window.toggleCart = () => {
    const modal = document.getElementById('cart-modal');
    modal.style.display = modal.style.display === 'block' ? 'none' : 'block';
  };

  window.toggleCheckout = () => {
    const modal = document.getElementById('checkout-form');
    modal.style.display = modal.style.display === 'block' ? 'none' : 'block';
  };

  window.checkout = () => {
    toggleCart();
    toggleCheckout();
  };

  window.clearCart = () => {
    cart = [];
    updateCart();
  };

  document.getElementById('payment-form').addEventListener('submit', (e) => {
    e.preventDefault();
    alert('Pago realizado con éxito');
    cart = [];
    updateCart();
    toggleCheckout();
  });
});
