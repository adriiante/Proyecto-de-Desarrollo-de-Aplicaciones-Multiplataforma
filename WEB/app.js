document.addEventListener('DOMContentLoaded', () => {
  const cartCountElement = document.getElementById('cart-count');
  const cartItemsElement = document.getElementById('cart-items');
  let cart = [];

  function updateCart() {
    cartCountElement.textContent = cart.length;
    cartItemsElement.innerHTML = '';
    cart.forEach(item => {
      const div = document.createElement('div');
      div.textContent = `${item.name} - $${item.price}`;
      cartItemsElement.appendChild(div);
    });
  }

  document.querySelectorAll('.producto button').forEach(button => {
    button.addEventListener('click', () => {
      const producto = button.closest('.producto');
      const name = producto.querySelector('h2').textContent;
      const price = parseFloat(producto.querySelector('.precio').textContent.replace('$', ''));
      cart.push({ name, price });
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

  document.getElementById('payment-form').addEventListener('submit', (e) => {
    e.preventDefault();
    alert('Pago realizado con éxito');
    cart = [];
    updateCart();
    toggleCheckout();
  });
});
