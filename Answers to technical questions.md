# Answers

<h2>Resilience side-effects</h2>

    Is it possible that even when order service responds to a request with a 201 status code response the api client sends the same request again -for example as part of a retry policy on the api client side?

No it isn't, because the retry policies generally will resend the request when the api responds with 428 or 5xx, and the developer must block to resend the request.

----------------------------------------------

<h2>From the message producer standpoint</h2>

    Is it possible to publish messages in a different order than the one in which their respective orders were processed and persisted? If so, how can we  avoid it? What trade-offs are considered?


Yes it is possible, because in a multi-threading scenario the request 2 could be faster than request 1. We can avoid it adding aggregates versioning, or blocking the request until the previous finished, the trade-off is worse performance.

    Is it possible to publish message duplicates?

Yes it is possible because you can resend the same request, we can avoid it informing MessageId(Azure Service Bus).

---------------------------------------------

<h2>From the message consumer standpoint</h2>

    Is it possible to consume messages out of order? If so, how can we avoid it?


Yes it is possible. We can avoid it versioning aggregates and persisting the last version consumed.

    Is it possible to consume message duplicates? If so, how can we avoid it?

Yes it is possible. We can avoid it persisting messages consumed.

----------------------------------------------

<h2>Message semantics</h2>

At the moment one of OrderCreatedEvent messages consumers is shipping service, which reacts to those messages
creating order shipments accordingly. In case we decide to produce ShipOrderCommand messages instead of
OrderConfirmedEvent ones

    Which are the semantic differences?

The semantic differences are when we send OrderConfirmedEvent the order have been confirmed, it refers to past, and when we send ShipOrderCommand we are saying the sytem must do the shipping, the present.

    Does service interaction change in a    meaningful way?

Not too much, because the result of each interaction is to ship the order.

    Is there a need to introduce additional components or change system topology?

You should add some compensations if any parts of the whole transaction fails.